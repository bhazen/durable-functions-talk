using System;
using System.Threading.Tasks;
using DurableFunctionsDemo.Models;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsDemo.Service.Unreliable
{
    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;

        public DataService(ILogger<DataService> logger)
        {
            _logger = logger;
        }

        public Task SaveReport(ExpenseReport expenseReport)
        {
            throw new Exception("Demo error");
        }

        public Task SaveReportReliable(ExpenseReport expenseReport)
        {
            _logger.LogInformation($"Saved info for report {expenseReport.Id}");

            return Task.CompletedTask;
        }
    }
}