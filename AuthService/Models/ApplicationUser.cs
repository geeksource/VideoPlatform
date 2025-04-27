using Microsoft.AspNetCore.Identity;

namespace AuthService.Models
{
    public class ApplicationUser: IdentityUser
    {
        public bool IsSubscribed { get; set; }
    }
}
