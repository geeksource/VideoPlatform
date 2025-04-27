namespace SubscriptionService.Models
{
    public class UserSubscription
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int SubscriptionPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = false;
        public string ReferenceId { get; set; }
        public SubscriptionPlan? SubscriptionPlan { get; set; }
    }

}
