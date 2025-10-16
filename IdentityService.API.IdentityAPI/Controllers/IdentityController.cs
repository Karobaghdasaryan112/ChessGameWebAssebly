using IdentityService.API.IdentityAPI.Contracts;
using IdentityService.Application.Features.MediatR.Requests.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;
using SharedResources.DTOs.IdentityDTOs.ResponseDTOs;
using SharedResources.Requests;
using SharedResources.Responses.ResponseMessages;

namespace IdentityService.API.IdentityAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;
        public IdentityController(IAuthService authService, IMediator mediator)
        {
            _mediator = mediator;
            _authService = authService;
        }
        [HttpPost]
        public IActionResult GetStatus()
        {
            return Ok(new { status = "Auth service is running." });
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDTO loginRequest)
        {
            var userRegistrationCommand = new UserSigninCommand<
                IRequestTypes<LoginDTO>,
                IResponseTypes<SignInDTO, IdentityResponseMesage>>
                (new IdentityRequest<LoginDTO>(loginRequest));

            return Ok(await _mediator.Send(userRegistrationCommand));
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegistrationAsync(RegistrationDTO registerRequest)
        {
            var userRegistrationCommand = new UserRegistrationCommand<
                IRequestTypes<RegistrationDTO>,
                IResponseTypes<RegistrationResponseDTO, IdentityResponseMesage>>
                (new IdentityRequest<RegistrationDTO>(registerRequest));

            return Ok(await _mediator.Send(userRegistrationCommand));
        }
    }
}
