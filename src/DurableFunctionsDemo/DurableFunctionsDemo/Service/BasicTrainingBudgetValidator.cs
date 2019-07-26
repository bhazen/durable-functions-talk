using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service
{
    public class BasicTrainingBudgetValidator : IExpenseValidator
    {
        public Task<bool> IsApproved(int employeeId, decimal amount) => Task.FromResult(employeeId % 2 == 0);
    }
}