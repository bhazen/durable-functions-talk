using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public Task SendReportCompletedNotification(int employeeId)
        {
            if (DateTime.UtcNow.Minute % 2 == 0)
            {
                throw new Exception("Demo error!");
            }

            return Task.CompletedTask;
        }

        public Task SendApprovalRequestNotification(int reportId)
        {
            _logger.LogInformation($"Sent approval request for report {reportId}");

            return Task.CompletedTask;
        }

        public Task SendApprovalEscalationNotification(int reportId)
        {
            _logger.LogInformation($"Escalated approval for report {reportId}");

            return Task.CompletedTask;
        }

        public Task SendReportReceivedResponseNotification(int reportId, string response)
        {
            _logger.LogInformation($"Expense report {reportId} received response {response}");

            return Task.CompletedTask;
        }
    }
}