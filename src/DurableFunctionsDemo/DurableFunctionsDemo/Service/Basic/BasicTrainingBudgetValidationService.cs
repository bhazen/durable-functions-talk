using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service.Basic
{
    public class BasicTrainingBudgetValidationService : IBasicExpenseValidationService
    {
        public Task<bool> IsApproved(int employeeId, decimal amount) => Task.FromResult(employeeId % 2 == 0);
    }
}