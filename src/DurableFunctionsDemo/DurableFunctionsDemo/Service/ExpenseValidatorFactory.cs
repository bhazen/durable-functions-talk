using DurableFunctionsDemo.Models;
using System;

namespace DurableFunctionsDemo.Service
{
    public class ExpenseValidatorFactory : IExpenseValidatorFactory
    {
        public IExpenseValidator Create(ExpenseCategory expenseCategory)
        {
            switch (expenseCategory)
            {
                case ExpenseCategory.Parking:
                    return new ParkingBudgetValidator();
                case ExpenseCategory.Software:
                    return new SoftwareBudgetValidator();
                case ExpenseCategory.Training:
                    return new BasicTrainingBudgetValidator();
                default:
                    throw new ArgumentException("Unsupported expense category", nameof(expenseCategory));
            }
        }
    }
}