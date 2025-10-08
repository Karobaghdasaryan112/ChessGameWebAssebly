using FluentValidation;
using IdentityService.API.IdentityAPI.Contracts;
using IdentityService.API.IdentityAPI.Services;
using IdentityService.Application.Features.MediatR.Base;
using IdentityService.Application.Features.MediatR.Handlers.Commands;
using IdentityService.Application.Validation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Application
{
    public static class ApplicationServicesRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IUserService, UserService>();

            services.AddValidatorsFromAssemblyContaining(typeof(LoginDTOValidator));

            services.AddLogging();

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(UserRegistrationCommandHandler).Assembly));

        }
    }
}
