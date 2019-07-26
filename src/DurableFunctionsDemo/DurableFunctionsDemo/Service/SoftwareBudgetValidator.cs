using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service
{
    public class SoftwareBudgetValidator : IExpenseValidator
    {
        public Task<bool> IsApproved(int employeeId, decimal amount) => Task.FromResult(false);
    }
}