namespace SubscriptionService.Dtos
{
    public class SubscribeRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; }
        public int PlanId { get; set; }
        public string ReferenceId { get; set; }
    }

}
