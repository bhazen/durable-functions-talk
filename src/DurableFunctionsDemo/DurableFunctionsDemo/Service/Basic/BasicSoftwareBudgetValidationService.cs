using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service.Basic
{
    public class BasicSoftwareBudgetValidationService : IBasicExpenseValidationService
    {
        public Task<bool> IsApproved(int employeeId, decimal amount) => Task.FromResult(false);
    }
}