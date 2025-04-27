using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using VideoPlatformPortal.Models;
using VideoPlatformPortal.ViewModels;

namespace VideoPlatformPortal.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly HttpClient _httpClient;
        private readonly ExteralServices _config;

        public SubscriptionService(HttpClient httpClient, IOptions<ExteralServices> options)
        {
            _httpClient = httpClient;
            _config = options.Value;
        }

        public async Task<List<SubscriptionPlanViewModel>> GetPlansAsync()
        {
            string Url = $"{_config.SubscriptionService}/plans";
            var response = await _httpClient.GetFromJsonAsync<List<SubscriptionPlanViewModel>>(Url);
            return response ?? new List<SubscriptionPlanViewModel>();
        }

        public async Task<string> GeneratePaymentLinkAsync(string userId,string userEmail,string planId)
        {
            var request = new
            {
                UserId = userId,
                PlanId = planId,
                UserEmail= userEmail,
                ReferenceId = Guid.NewGuid().ToString()
            };
            string ReqUrl = $"{_config.SubscriptionService}/Subscribe";
            var response = await _httpClient.PostAsJsonAsync($"{_config.SubscriptionService}/Subscribe", request);
            var result = await response.Content.ReadFromJsonAsync<CheckoutResponse>();
            return result?.CheckoutUrl!;
        }
        public async Task<List<SubscriptionPlanViewModel>> GetActivePlansAsync(string userId)
        {
            string Url = $"{_config.SubscriptionService}/GetActivePlan?UserId={userId}";
            var response = await _httpClient.GetFromJsonAsync<List<SubscriptionPlanViewModel>>(Url);
            return response ?? new List<SubscriptionPlanViewModel>();
        }

        

        private class CheckoutResponse
        {
            public string CheckoutUrl { get; set; }
        }
    }

}
