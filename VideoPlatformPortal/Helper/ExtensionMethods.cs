using System.Security.Claims;
using System.Security.Principal;

namespace VideoPlatformPortal.Helper
{
    public static class ExtensionMethods
    {
        public static string GetEmail(this ClaimsPrincipal _user)
        {
            if (_user == null)
            {
                return string.Empty;
            }
            var userEmail = _user.Claims.Where(c => c.Type == ClaimTypes.Email.ToString()).FirstOrDefault();
            return userEmail?.Value;
        }
        public static string GetUserId(this ClaimsPrincipal _user)
        {
            if (_user == null)
            {
                return string.Empty;
            }
            var userId = _user.Claims.Where(c => c.Type == "UserId").FirstOrDefault();
            return userId?.Value;
        }
        public static void UpdateClaims(this ClaimsPrincipal _user,string Key,string value, HttpContext context)
        {
            if (_user == null)
                return;

            var identity = (ClaimsIdentity)_user.Identity;

            // Remove the existing claim if it exists
            var claimToRemove = identity.FindFirst(Key);
            if (claimToRemove != null)
            {
                identity.RemoveClaim(claimToRemove);
            }

            // Add the new claim
            identity.AddClaim(new Claim(Key, value));

            // Create a new ClaimsPrincipal with the updated identity
            var updatedUser = new ClaimsPrincipal(identity);

            // Update the HttpContext.User with the new ClaimsPrincipal
            context.User = updatedUser;
        }
        public static List<string> GetActiveSubscriptions(this ClaimsPrincipal _user)
        {
            if (_user == null)
                return new();
            var subscription = _user.Claims.Where(c => c.Type == "ActiveSubscriptions").Select(c => c.Value).FirstOrDefault();
            if(subscription == null)
                return new();
            return subscription.Split(",").ToList();
        }

    }
}
