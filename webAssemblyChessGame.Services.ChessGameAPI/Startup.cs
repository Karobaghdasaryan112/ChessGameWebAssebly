using ChessGame.Core.Services;
using ChessGame.Core.Services.MediatR.Handlers.Commands;
using ChessGame.Core.Services.MediatR.Handlers.Queries;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Infrastructure.Infrastructure;
using ChessGame.Infrastructure.Infrastructure.Hubs;
using ChessGame.Infrastructure.Infrastructure.Persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ChessService.API.ChessGameAPI
{
    public class Startup(IConfiguration configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });



            services.AddCoreServices(configuration);

            services.AddPersistanceServices(configuration);

            services.AddInfrastructureServices(configuration);

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
            app.UseEndpoints(endpoint =>
            {
                endpoint.MapControllers();
                endpoint.MapHub<GameHub>("/gameHub");
            });

        }
    }
}
