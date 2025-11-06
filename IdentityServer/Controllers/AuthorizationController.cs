using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using SharedResources.Contracts.DTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using System.Net;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Controllers
{
    [ApiController]
    [Route("connect/[action]")]

    public class AuthorizationController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        public AuthorizationController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }
        public async Task<IActionResult> authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request == null)
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    IdentityResponse<IIdentityResponseDTO>.
                        CreateErrorResponse(IdentityResponseMesage.InternalServerError, HttpStatusCode.InternalServerError, new())
                        );

            var claimsIdentity = new List<ClaimsIdentity>();
            claimsIdentity.Add(new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role));

            var principal = new ClaimsPrincipal(
                claimsIdentity);
            principal.SetClaim(Claims.Subject, User.Identity?.Name! ?? "NoName");

            return SignIn(principal,OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        }

    }
}
