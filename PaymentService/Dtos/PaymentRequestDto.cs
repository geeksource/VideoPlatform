namespace PaymentService.Dtos
{
    public class PaymentRequestDto
    {
        public string Email { get; set; }
        public string PlanName { get; set; }
        public long Amount { get; set; } // e.g., 1000 for $10.00
    }
}
