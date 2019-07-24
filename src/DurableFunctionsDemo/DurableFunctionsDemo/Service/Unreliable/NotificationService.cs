using System;
using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service.Unreliable
{
    public class NotificationService : INotificationService
    {
        public Task SendReportCompletedNotification(int employeeId)
        {
            if (DateTime.UtcNow.Minute % 2 == 0)
            {
                throw new Exception("Demo error!");
            }

            return Task.CompletedTask;
        }
    }
}