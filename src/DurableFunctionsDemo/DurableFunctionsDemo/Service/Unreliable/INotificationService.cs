using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service.Unreliable
{
    public interface INotificationService
    {
        Task SendReportCompletedNotification(int employeeId);
    }
}