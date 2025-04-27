using VideoPlatformPortal.ViewModels;

namespace VideoPlatformPortal.Services
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionPlanViewModel>> GetPlansAsync();
        Task<string> GeneratePaymentLinkAsync(string userId, string userEmail, string planId);
        Task<List<SubscriptionPlanViewModel>> GetActivePlansAsync(string userId);
    }

}
