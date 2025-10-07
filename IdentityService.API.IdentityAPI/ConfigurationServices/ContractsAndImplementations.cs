using IdentityService.API.IdentityAPI.Contracts;
using IdentityService.API.IdentityAPI.Services;

namespace IdentityService.API.IdentityAPI.ConfigurationServices
{
    public static class ContractsAndImplementations
    {
        public static void AddContractsAndImplementations(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
