using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service
{
    public interface IExpenseValidator
    {
        Task<bool> IsApproved(int employeeId, decimal amount);
    }
}