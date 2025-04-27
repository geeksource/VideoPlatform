using AuthService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Text;

namespace AuthService.Data
{
    public static class IoC
    {
        public static IServiceCollection AddDbContextConfig(this IServiceCollection services,IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            return services;
        }
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services)
        {            
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            return services;
        }
        public static IServiceCollection AddJwtAuthenticationAndAuthorizationConfig(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer=false,
                        ValidateAudience=false,
                        ValidateLifetime=true,
                        ValidateIssuerSigningKey=true,
                        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyForJWT123"))
                    };
                });
            services.AddAuthentication();
            return services;
        }
    }
}
