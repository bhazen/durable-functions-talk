using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DurableFunctionsDemo.Models;
using DurableFunctionsDemo.Service;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsDemo
{
    public class BetterExpenseReportHandling
    {
        private readonly IExpenseValidatorFactory _expenseValidatorFactory;
        private readonly IDataService _dataService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BetterExpenseReportHandling> _logger;

        public BetterExpenseReportHandling(IExpenseValidatorFactory expenseValidatorFactory, IDataService dataService, INotificationService notificationService, ILogger<BetterExpenseReportHandling> logger)
        {
            _expenseValidatorFactory = expenseValidatorFactory;
            _dataService = dataService;
            _notificationService = notificationService;
            _logger = logger;
        }

        [FunctionName(nameof(BetterExpenseReportHandling))]
        public async Task BetterHandlingOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
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
                lineItemTasks.Add(context.CallActivityAsync<CheckApprovalResult>(nameof(BetterHandlingValidateReportLineItem), approvalRequest));
            }

            await Task.WhenAll(lineItemTasks);
            foreach(var lineItem in report.LineItems)
            {
                lineItem.Approved = lineItemTasks.FirstOrDefault(task => task.Result.LineItemId == lineItem.Id)?.Result.IsApproved ?? false;
            }

            try
            {
                await context.CallActivityAsync(nameof(SaveReport), report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save validated report");
            }

            var retryOptions = new RetryOptions(TimeSpan.FromSeconds(5), 3);
            await context.CallActivityWithRetryAsync(nameof(NotifyUserReportFinished), retryOptions, report.EmployeeId);
        }

        [FunctionName(nameof(BetterHandlingValidateReportLineItem))]
        public async Task<CheckApprovalResult> BetterHandlingValidateReportLineItem([ActivityTrigger] CheckApprovalRequest checkApprovalRequest)
        {
            var validator = _expenseValidatorFactory.Create(checkApprovalRequest.ExpenseCategory);

            var approved = await validator.IsApproved(checkApprovalRequest.EmployeeId, checkApprovalRequest.Amount);

            return new CheckApprovalResult { LineItemId = checkApprovalRequest.LineItemId, IsApproved = approved };
        }

        [FunctionName(nameof(SaveReport))]
        public async Task SaveReport([ActivityTrigger] ExpenseReport expenseReport)
        {
            await _dataService.SaveReport(expenseReport);
        }

        [FunctionName(nameof(NotifyUserReportFinished))]
        public async Task NotifyUserReportFinished([ActivityTrigger] int employeeId)
        {
            await _notificationService.SendReportCompletedNotification(employeeId);
        }

        [FunctionName(nameof(BetterHandlingStart))]
        public async Task<HttpResponseMessage> BetterHandlingStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [DurableClient]IDurableOrchestrationClient starter,
            ILogger log)
        {
            var expenseReport = await req.Content.ReadAsAsync<ExpenseReport>();

            string instanceId = await starter.StartNewAsync(nameof(BetterExpenseReportHandling), expenseReport);

            _logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}