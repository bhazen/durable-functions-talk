using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service.Basic
{
    public class BasicParkingBudgetValidationService : IBasicExpenseValidationService
    {
        public Task<bool> IsApproved(int employeeId, decimal amount) => Task.FromResult(true);
    }
}