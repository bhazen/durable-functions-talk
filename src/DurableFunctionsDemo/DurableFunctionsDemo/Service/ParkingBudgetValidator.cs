using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service
{
    public class ParkingBudgetValidator : IExpenseValidator
    {
        public Task<bool> IsApproved(int employeeId, decimal amount) => Task.FromResult(true);
    }
}