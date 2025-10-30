using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace IdentityServer.Controllers
{
    [ApiController]
    [Route("connect/[action]")]

    public class AuthorizationController : ControllerBase
    {
        private static Dictionary<string, AuthCodeRecord> AuthorizationCodes = new();
        private readonly IServiceProvider _serviceProvider;
        public AuthorizationController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }
        public async Task<IActionResult> authorize(
     [FromQuery] string? client_id,
     [FromQuery] string redirect_uri,
     [FromQuery] string? response_type,
     [FromQuery] string? response_mode,
     [FromQuery] string? scope,
     [FromQuery] string? state,
     [FromQuery(Name = "code_challenge")] string? code_challenge,
     [FromQuery(Name = "code_challenge_method")] string? code_challenge_method
 )
        {
            if (
                string.IsNullOrEmpty(client_id) ||
                string.IsNullOrEmpty(redirect_uri) ||
                string.IsNullOrEmpty(scope) ||
                string.IsNullOrEmpty(response_type)
            )
                return BadRequest("Missing required Parameters");

            var requestedScopes = scope?
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToArray()
                ?? Array.Empty<string>();

            if (!string.IsNullOrEmpty(code_challenge) &&
               code_challenge_method != null &&
               code_challenge_method != "plain" &&
               code_challenge_method != "S256")
            {
                return BadRequest("Invalid code_challenge_method.");
            }

            string authorizationCode = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace("+", "-").Replace("/", "_").TrimEnd('=');

            using (var serviceScope = _serviceProvider.CreateAsyncScope())
            {
                var openIddictApplications = serviceScope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
                var openIddictScopes = serviceScope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

                if (await openIddictApplications.FindByClientIdAsync(client_id) is null)
                    return BadRequest("Invalid Client_id");

                var validationResult = openIddictScopes.FindByNamesAsync(requestedScopes.ToImmutableArray());
                if (validationResult.ToBlockingEnumerable().Count() != requestedScopes.Count())
                    return BadRequest("Invalid scopes");
            }

            var codeVerifier = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace("+", "-").Replace("/", "_").TrimEnd('=');

            string storedCodeChallenge = null;
            if (code_challenge_method == "S256")
            {
                using (var sha256 = SHA256.Create())
                {
                    var challengeBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
                    storedCodeChallenge = Convert.ToBase64String(challengeBytes)
                        .Replace("+", "-").Replace("/", "_").TrimEnd('=');
                }
            }
            else
            {
                storedCodeChallenge = code_challenge;
            }

            AuthorizationCodes[authorizationCode] = new AuthCodeRecord
            {
                ClientId = client_id,
                RedirectUri = redirect_uri,
                Scope = scope,
                ResponseMode = response_mode,
                ResponseType = response_type,
                CodeChallenge = storedCodeChallenge,
                CodeChallengeMethod = code_challenge_method,
                CreationTime = DateTime.UtcNow,
                CodeVerifier = codeVerifier
            };

            var uri = $"{redirect_uri}?code={HttpUtility.UrlEncode(authorizationCode)}&state={HttpUtility.UrlEncode(state)}";
            return Redirect(uri);
        }

        private class AuthCodeRecord
        {
            public string ClientId { get; set; } = default!;
            public string RedirectUri { get; set; } = default!;
            public string Scope { get; set; } = default!;
            public string ResponseMode { get; set; } = default!;
            public string ResponseType { get; set; } = default!;
            public string? CodeChallenge { get; set; }
            public string? CodeChallengeMethod { get; set; }
            public string CodeVerifier { get; set; } = default!;
            public DateTime CreationTime { get; set; }
        }
    }
}
