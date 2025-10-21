using IdentityService.API.IdentityAPI.Helpers;
using IdentityService.Domain.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Persistance
{
    public static class PersistanceServicesRegistration
    {
        public static IServiceCollection AddPersistanceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                //.AddApplicationCookie()
                //.Configure(options =>
                //{
                //    options.LoginPath = "/Login";
                //    options.SlidingExpiration = true;
                //    options.AccessDeniedPath = "/Registration";
                //    options.Cookie.Name = ".YourAppAuth";
                //    options.Cookie.HttpOnly = true;
                //    options.Cookie.SameSite = SameSiteMode.None;
                //})
                ;

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login";
                options.SlidingExpiration = true;
                options.AccessDeniedPath = "/Registration";
                options.Cookie.Name = ".YourAppAuth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.Domain = "localhost:7225";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                options.Cookie.Expiration = TimeSpan.FromDays(14);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<IdentityContext>()
                    .AddDefaultTokenProviders();
            return services;
        }
    }
}
