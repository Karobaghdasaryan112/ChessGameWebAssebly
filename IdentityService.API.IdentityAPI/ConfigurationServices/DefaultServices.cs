namespace IdentityService.API.IdentityAPI.ConfigurationServices
{
    public static class DefaultServices
    {
        public static void AddDefaultServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddAuthentication();
            services.AddAuthorizationCore();
            services.AddHttpContextAccessor();

        }
    }
}
