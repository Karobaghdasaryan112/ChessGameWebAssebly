using IdentityService.API.IdentityAPI.Contracts;
using Microsoft.AspNetCore.Mvc;
using SharedResources.DTOs.IdentityDTOs;
using SharedResources.Responses;

namespace IdentityService.API.IdentityAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IAuthService _authService;
        public IdentityController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost]
        public IActionResult GetStatus()
        {
            return Ok(new { status = "Auth service is running." });
        }
        [HttpPost("login")]
        public Task<IActionResult> LoginAsync(LoginDTO loginRequest)
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                return Task.FromResult<IActionResult>(Ok(new { message = "User is already authenticated." }));
            }
            return null;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegistrationAsync(RegistrationDTO registerRequest)
        {
            await _authService.CreateUserAsync(registerRequest);
            return Ok(new { response = IdentityResponse<CreateUserDTO>.identityResponse });
        }
    }
}
