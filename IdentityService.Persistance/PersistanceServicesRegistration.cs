using IdentityService.API.IdentityAPI.Helpers;
using IdentityService.Domain.Domain;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityService.Persistance
{
    public static class PersistanceServicesRegistration
    {
        public static IServiceCollection AddPersistanceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            var jwtSettings = configuration.Get<JwtSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = BearerTokenDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = BearerTokenDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                var jwtSettings = configuration.Get<JwtSettings>();
                var securityKeyAsBytes = Encoding.UTF8.GetBytes(jwtSettings?.SecurityKey!);

                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSettings?.Issuer!,
                    ValidAudience = jwtSettings?.Audience!,
                    RequireExpirationTime = false,
                    IssuerSigningKey = new SymmetricSecurityKey(securityKeyAsBytes)
                };
            });

                services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()  
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
