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
            })
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost:7287"; 
                    options.Audience = "http://localhost:7287";
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "http://localhost:7287",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("6FJTnf91RFneNeVRNY3Fpjb2/bDLx7jfJMOSyv36O4QMr9NoqgvbDytT0TPtzea1ACLA4NJZLHK2w3CwgpUjxQ==")),
                        ValidateAudience = true,
                        ValidAudience = "http://localhost:7287",
                        ValidateLifetime = true
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                                context.Token = accessToken;

                            return Task.CompletedTask;
                        }
                    };
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
