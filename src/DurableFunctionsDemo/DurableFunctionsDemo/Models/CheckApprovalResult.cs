namespace DurableFunctionsDemo.Models
{
    public class CheckApprovalResult
    {
        public int LineItemId { get; set; }

        public bool IsApproved { get; set; }
    }
}