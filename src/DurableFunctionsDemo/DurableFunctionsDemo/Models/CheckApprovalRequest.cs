namespace DurableFunctionsDemo.Models
{
    public class CheckApprovalRequest
    {
        public int LineItemId { get; set; }

        public int EmployeeId { get; set; }

        public decimal Amount { get; set; }

        public ExpenseCategory ExpenseCategory { get; set; }
    }
}