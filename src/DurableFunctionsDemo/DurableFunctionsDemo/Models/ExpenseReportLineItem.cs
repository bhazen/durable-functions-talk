using System;

namespace DurableFunctionsDemo.Models
{
    public class ExpenseReportLineItem
    {
        public int Id { get; set; }

        public ExpenseCategory Category { get; set; }

        public decimal Amount { get; set; }

        public DateTime DateIncurred { get; set; }

        public string Description { get; set; }

        public bool Approved { get; set; }
    }
}