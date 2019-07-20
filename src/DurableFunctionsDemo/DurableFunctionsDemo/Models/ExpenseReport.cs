using System;
using System.Collections.Generic;

namespace DurableFunctionsDemo.Models
{
    public class ExpenseReport
    {
        public DateTime DateSubmitted { get; set; }

        public int EmployeeId { get; set; }

        public IEnumerable<ExpenseReportLineItem> LineItems { get; set; }
    }
}