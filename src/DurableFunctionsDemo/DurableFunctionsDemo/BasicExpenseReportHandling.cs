using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DurableFunctionsDemo.Models;
using DurableFunctionsDemo.Service.Basic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsDemo
{
    public class BasicExpenseReportHandling
    {
        private IBasicExpenseValidationProvider _basicExpenseValidationProvider;
        private ILogger<BasicExpenseReportHandling> _logger;

        public BasicExpenseReportHandling(IBasicExpenseValidationProvider basicExpenseValidationProvider, ILogger<BasicExpenseReportHandling> logger)
        {
            _basicExpenseValidationProvider = basicExpenseValidationProvider;
            _logger = logger;
        }

        [FunctionName(nameof(BasicHandlingOrchestrator))]
        public async Task BasicHandlingOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var report = context.GetInput<ExpenseReport>();

            var lineItemTasks = new List<Task<bool>>();
            foreach(var lineItem in report.LineItems)
            {
                lineItemTasks.Add(context.CallActivityAsync<bool>(nameof(ValidateReportLineItem), (report.EmployeeId, lineItem.Amount, lineItem.Category)));
            }

            await Task.WhenAll(lineItemTasks);
        }

        [FunctionName(nameof(ValidateReportLineItem))]
        public async Task<bool> ValidateReportLineItem([ActivityTrigger] DurableActivityContext context)
        {
            var (id, amount, category) = context.GetInput<(int, decimal, ExpenseCategory)>();
            var validationService = _basicExpenseValidationProvider.GetExpenseValidationService(category);

            return await validationService.IsApproved(id, amount);
        }

        [FunctionName(nameof(BasicHandlingStart))]
        public async Task<HttpResponseMessage> BasicHandlingStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter)
        {
            var expenseReport = await req.Content.ReadAsAsync<ExpenseReport>();

            string instanceId = await starter.StartNewAsync(nameof(BasicHandlingOrchestrator), expenseReport);

            _logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}