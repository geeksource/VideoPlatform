using VideoPlatformPortal.ViewModels;

namespace VideoPlatformPortal.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterModel model);
        Task<string> LoginAsync(LoginModel model);
    }
}
