using IdentityService.API.IdentityAPI.ConfigurationServices;
using IdentityService.Application;
using IdentityService.Persistance;

namespace IdentityService.API.IdentityAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices();

            services.AddPersistanceServices(Configuration);

            services.AddCorsServices();

            services.AddDefaultServices(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseCors("AllowGateway_CORS");
        }
    }
}
