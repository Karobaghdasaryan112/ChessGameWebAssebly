namespace IdentityService.API.IdentityAPI.ConfigurationServices
{
    public static class CORS_Services
    {
        public static void AddCorsServices(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowGateway_CORS", policy =>
                {
                    policy.WithOrigins("http://localhost:7185")
                          .AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
        }
    }   
}
