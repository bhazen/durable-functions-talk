using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service.Basic
{
    public interface IBasicExpenseValidationService
    {
        Task<bool> IsApproved(int employeeId, decimal amount);
    }
}