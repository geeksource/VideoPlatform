using System.IdentityModel.Tokens.Jwt;

namespace VideoPlatformPortal.Helper
{
    public static class JwtHelper
    {
        public static string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "userId" || c.Type == "UserId");

            return userIdClaim?.Value;
        }
    }
}
