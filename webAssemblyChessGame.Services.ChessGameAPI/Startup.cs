using ChessGame.Core.Services;
using ChessGame.Infrastructure.Infrastructure;
using ChessGame.Infrastructure.Infrastructure.Hubs;
using ChessGame.Infrastructure.Infrastructure.Persistance;
using ChessService.API.ChessGameAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ChessService.API.ChessGameAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                    .AllowAnyOrigin();
                });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
 

            services.AddCoreServices(_configuration);

            services.AddPersistanceServices(_configuration);

            services.AddInfrastructureServices(_configuration);

            services.AddControllers();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            services.AddLogging();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseEndpoints(endpoint =>
            {
                endpoint.MapControllers();
                endpoint.MapHub<GameHub>("/gameHub");
            });
        }
    }
}
