using DurableFunctionsDemo.Models;

namespace DurableFunctionsDemo.Service.Basic
{
    public interface IBasicExpenseValidationProvider
    {
        IBasicExpenseValidationService GetExpenseValidationService(ExpenseCategory expenseCategory);
    }
}