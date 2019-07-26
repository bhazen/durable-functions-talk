using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DurableFunctionsDemo.Models;
using DurableFunctionsDemo.Service.Basic;
using DurableFunctionsDemo.Service.Unreliable;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsDemo
{
    public class AdvancedExpenseReportHandling
    {
        private readonly IBasicExpenseValidationProvider _basicExpenseValidationProvider;
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AdvancedExpenseReportHandling> _logger;

        public AdvancedExpenseReportHandling(IBasicExpenseValidationProvider basicExpenseValidationProvider, IDataService dataService, INotificationService notificationService, ILogger<AdvancedExpenseReportHandling> logger)
        {
            _basicExpenseValidationProvider = basicExpenseValidationProvider;
            _dataService = dataService;
            _notificationService = notificationService;
            _logger = logger;
        }

        [FunctionName(nameof(AdvancedHandlingOrchestrator))]
        public async Task AdvancedHandlingOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            try
            {
                var report = context.GetInput<ExpenseReport>();

                var lineItemTasks = new List<Task<CheckApprovalResult>>();
                foreach (var lineItem in report.LineItems)
                {
                    var approvalRequest = new CheckApprovalRequest
                    {
                        LineItemId = lineItem.Id,
                        EmployeeId = report.EmployeeId,
                        Amount = lineItem.Amount,
                        ExpenseCategory = lineItem.Category
                    };
                    lineItemTasks.Add(context.CallActivityAsync<CheckApprovalResult>(nameof(AdvancedHandlingValidateReportLineItem), approvalRequest));
                }

                await Task.WhenAll(lineItemTasks);
                foreach (var lineItem in report.LineItems)
                {
                    lineItem.Approved = lineItemTasks.FirstOrDefault(task => task.Result.LineItemId == lineItem.Id)?.Result.IsApproved ?? false;
                }

                await context.CallActivityAsync(nameof(SaveExpenseReport), report);
                await context.CallActivityAsync(nameof(SendApprovalRequestNotification), report.Id);

                using (var timeoutCts = new CancellationTokenSource())
                {
                    var expiration = context.CurrentUtcDateTime.AddHours(24);
                    var timeoutTask = context.CreateTimer(expiration, timeoutCts.Token);
                    var approvedTask = context.WaitForExternalEvent<string>(Constants.Events.Approval);

                    var resultTask = await Task.WhenAny(timeoutTask, approvedTask);
                    if (resultTask == timeoutTask)
                    {
                        await context.CallActivityAsync(nameof(SendEscalationNotification), report.Id);
                    }
                    else
                    {
                        await context.CallActivityAsync(nameof(SendReportReceivedResponseNotification), (report.Id, approvedTask.Result));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred during orchestration {context.InstanceId}");

                // In a real application we'd probably want to send some sort of notification here
            }
        }

        [FunctionName(nameof(AdvancedHandlingValidateReportLineItem))]
        public async Task<CheckApprovalResult> AdvancedHandlingValidateReportLineItem([ActivityTrigger] CheckApprovalRequest checkApprovalRequest)
        {
            var validationService = _basicExpenseValidationProvider.GetExpenseValidationService(checkApprovalRequest.ExpenseCategory);

            var approved = await validationService.IsApproved(checkApprovalRequest.EmployeeId, checkApprovalRequest.Amount);

            return new CheckApprovalResult { LineItemId = checkApprovalRequest.LineItemId, IsApproved = approved };
        }

        [FunctionName(nameof(SaveExpenseReport))]
        public async Task SaveExpenseReport([ActivityTrigger] ExpenseReport expenseReport)
        {
            await _dataService.SaveReportReliable(expenseReport);
        }

        [FunctionName(nameof(SendApprovalRequestNotification))]
        public async Task SendApprovalRequestNotification([ActivityTrigger] int reportId)
        {
            await _notificationService.SendApprovalRequestNotification(reportId);
        }

        [FunctionName(nameof(SendEscalationNotification))]
        public async Task SendEscalationNotification([ActivityTrigger] int reportId)
        {
            await _notificationService.SendApprovalEscalationNotification(reportId);
        }

        [FunctionName(nameof(SendReportReceivedResponseNotification))]
        public async Task SendReportReceivedResponseNotification([ActivityTrigger] DurableActivityContext context)
        {
            var (reportId, response) = context.GetInput<(int, string)>();

            await _notificationService.SendReportReceivedResponseNotification(reportId, response);
        }

        [FunctionName(nameof(AdvancedHandlingStart))]
        public async Task<HttpResponseMessage> AdvancedHandlingStart(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
           [OrchestrationClient]DurableOrchestrationClient starter)
        {
            var expenseReport = await req.Content.ReadAsAsync<ExpenseReport>();

            string instanceId = await starter.StartNewAsync(nameof(AdvancedHandlingOrchestrator), expenseReport);

            _logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}