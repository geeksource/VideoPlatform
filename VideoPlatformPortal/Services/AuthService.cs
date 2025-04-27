
using System.Text.Json;
using System.Text;
using VideoPlatformPortal.Models;
using VideoPlatformPortal.ViewModels;
using Microsoft.Extensions.Options;

namespace VideoPlatformPortal.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ExteralServices _exteralServices;
        public AuthService(HttpClient httpClient, IOptions<ExteralServices> externalServices)
        {
            _httpClient = httpClient;
            _exteralServices = externalServices.Value;
        }
        public async Task<string> LoginAsync(LoginModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_exteralServices.AuthService}/login", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(); // Return the JWT token
            }

            return null;
        }


        public async Task<bool> RegisterAsync(RegisterModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_exteralServices.AuthService}/register", content);

            return response.IsSuccessStatusCode;
        }
    }
}
