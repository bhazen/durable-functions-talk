using System;

namespace DurableFunctionsDemo.Models
{
    public class ExpenseReportLineItem
    {
        public ExpenseCategory Category { get; set; }

        public decimal Amount { get; set; }

        public DateTime DateIncurred { get; set; }

        public string Description { get; set; }
    }
}