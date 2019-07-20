using DurableFunctionsDemo.Models;
using System;

namespace DurableFunctionsDemo.Service.Basic
{
    public class BasicExpenseValidationProvider : IBasicExpenseValidationProvider
    {
        public IBasicExpenseValidationService GetExpenseValidationService(ExpenseCategory expenseCategory)
        {
            switch (expenseCategory)
            {
                case ExpenseCategory.Parking:
                    return new BasicParkingBudgetValidationService();
                case ExpenseCategory.Software:
                    return new BasicSoftwareBudgetValidationService();
                case ExpenseCategory.Training:
                    return new BasicTrainingBudgetValidationService();
                default:
                    throw new ArgumentException("Unsupported expense category", nameof(expenseCategory));
            }
        }
    }
}