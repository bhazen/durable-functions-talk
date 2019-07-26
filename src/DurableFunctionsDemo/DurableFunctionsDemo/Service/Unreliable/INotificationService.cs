using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service.Unreliable
{
    public interface INotificationService
    {
        Task SendApprovalEscalationNotification(int reportId);
        Task SendApprovalRequestNotification(int reportId);
        Task SendReportCompletedNotification(int employeeId);
        Task SendReportReceivedResponseNotification(int reportId, string response);
    }
}