using IdentityService.API.IdentityAPI.Helpers;
using IdentityService.Domain.Domain;
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
            {
                options.UseOpenIddict();
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            var jwtSettings = configuration.Get<JwtSettings>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            services.AddOpenIddict()
                .AddCore(openIdBuilder =>
                {
                    openIdBuilder.UseEntityFrameworkCore().UseDbContext<IdentityContext>();
                })
                .AddServer(serverBuilder =>
                {
                    //serverBuilder.AllowPasswordFlow(); TO DO:

                    serverBuilder.AllowRefreshTokenFlow();

                    serverBuilder.AllowClientCredentialsFlow();

                    serverBuilder.SetTokenEndpointUris("/connect/Token");

                    serverBuilder.SetAuthorizationEndpointUris("/connect/authorize");

                    serverBuilder.SetUserInfoEndpointUris("connect/userinfo");


                    serverBuilder
                    .UseAspNetCore()
                    .EnableUserInfoEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableAuthorizationEndpointPassthrough();


                    serverBuilder
                        .AddDevelopmentSigningCertificate()
                        .AddDevelopmentEncryptionCertificate();

                    serverBuilder.AddSigningKey(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecurityKey!)));

                })
                .AddValidation(validationBuilder =>
                {
                    validationBuilder.UseLocalServer();
                    validationBuilder.UseAspNetCore();
                });


            //var jwtSettings = configuration.Get<JwtSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });

            return services;
        }
    }
}
