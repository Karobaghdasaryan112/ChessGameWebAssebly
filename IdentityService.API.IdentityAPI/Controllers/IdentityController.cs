using IdentityService.API.IdentityAPI.Contracts;
using IdentityService.Application.Features.MediatR.Requests.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;
using SharedResources.DTOs.IdentityDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace IdentityService.API.IdentityAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpClientFactory _httpClient;
        private readonly IMediator _mediator;
        public IdentityController(IAuthService authService, IMediator mediator, IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
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
                LoginDTO,
                ResponseDTO<SignInDTO, IdentityResponseMesage>>
                (loginRequest);

            var loginResult = await _mediator.Send(userRegistrationCommand);

            return Ok(await _mediator.Send(userRegistrationCommand));
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegistrationAsync(RegistrationDTO registerRequest)
        {
           var userRegistrationCommand = new UserRegistrationCommand<
                RegistrationDTO,
                ResponseDTO<RegistrationResponseDTO, IdentityResponseMesage>>
                (registerRequest);

            return Ok(await _mediator.Send(userRegistrationCommand));
        }
    }
}
