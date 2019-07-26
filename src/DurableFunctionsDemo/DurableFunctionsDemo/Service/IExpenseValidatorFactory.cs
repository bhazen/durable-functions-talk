using DurableFunctionsDemo.Models;

namespace DurableFunctionsDemo.Service
{
    public interface IExpenseValidatorFactory
    {
        IExpenseValidator Create(ExpenseCategory expenseCategory);
    }
}