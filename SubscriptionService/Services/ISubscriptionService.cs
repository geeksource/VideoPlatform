using SubscriptionService.Dtos;
using SubscriptionService.Models;

namespace SubscriptionService.Services
{
    public interface ISubscriptionService
    {
        Task<bool> SubscribeAsync(SubscribeRequestDto dto);
        Task<List<SubscriptionPlan>> GetPlansAsync();
        Task<SubscriptionPlan> GetPlansAsync(int Id);
        Task<bool> ActivateSubscription(string ReferenceId);

        Task<List<SubscriptionPlan>> GetActivePlan(string UserId);
    }

}
