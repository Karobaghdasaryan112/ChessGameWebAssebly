using IdentityService.API.IdentityAPI.Contracts;
using IdentityService.API.IdentityAPI.Helpers;
using IdentityService.Domain.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;
using SharedResources.DTOs.IdentityDTOs.ResponseDTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityService.API.IdentityAPI.Services
{

    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthService> _logger;
        private readonly JwtSettings _jwtSettings;
        private readonly IdentityContext identityContext;
        public AuthService(
            IdentityContext identityContext,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AuthService> logger,
            IOptions<JwtSettings> jwtSettings)
        {
            this.identityContext = identityContext;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task<SignInResult> PasswordSignInAsync(SignInDTO signInDTO)
        {
            var userName = signInDTO.userName;
            var password = signInDTO.password;
            var isPersistent = signInDTO.isPersistent;
            var lockoutOnFailure = signInDTO.lockoutOnFailure;
            return await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IResponseTypes<CreateUserDTO, IdentityResponseMesage>> CreateUserAsync(RegistrationDTO registrationDTO)
        {

            var failedResult = IdentityResponse<CreateUserDTO>.CreateErrorResponse(IdentityResponseMesage.UserCreationFailed, System.Net.HttpStatusCode.BadRequest, new List<string>());

            var IsExistingUser = await _userManager.FindByNameAsync(registrationDTO.userName);
            if (IsExistingUser != null)
            {
                return failedResult;
            }
            var applicationUser = new ApplicationUser
            {
                FirstName = registrationDTO.firstName,
                LastName = registrationDTO.lastName,
                UserName = registrationDTO.userName,
                Email = registrationDTO.email
            };
            applicationUser.PasswordHash = _userManager.PasswordHasher.HashPassword(applicationUser, registrationDTO.password);

            var result = await _userManager.CreateAsync(applicationUser, registrationDTO.password);
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e => failedResult.Errors.Add(e.Description));
                return failedResult;
            }
            var CreatingUser = await _userManager.FindByNameAsync(registrationDTO.userName);
            if (CreatingUser != null)
            {
                var responseDTO = new CreateUserDTO() { UserId = int.Parse(CreatingUser.Id) };


                return IdentityResponse<CreateUserDTO>.CreateSuccessResponse(responseDTO, IdentityResponseMesage.UserCreated, System.Net.HttpStatusCode.OK);
            }
            return IdentityResponse<CreateUserDTO>.CreateErrorResponse(IdentityResponseMesage.InternalServerError, System.Net.HttpStatusCode.InternalServerError, new List<string>());
        }

        public async Task<IResponseTypes<SignInDTO, IdentityResponseMesage>> LoginAsync(LoginDTO loginDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDTO.email, loginDTO.password, false, false);
            if (!result.Succeeded)
            {
                return  IdentityResponse<SignInDTO>.CreateErrorResponse(IdentityResponseMesage.UserSignInFailed, System.Net.HttpStatusCode.Unauthorized, new List<string>());
            }
            var user = await _userManager.FindByNameAsync(loginDTO.email);
            if (user != null)
            {
                var refreshTokenResult = await RefreshTokenAsync(new RefreshTokenDTO { RefreshToken = user.RefreshToken });
                if (refreshTokenResult.IsSuccess && refreshTokenResult.Data != null)
                {
                    var responseDTO = new SignInDTO
                    {
                        userName = refreshTokenResult.Data.userName,
                        isPersistent = true,
                        AccessToken = refreshTokenResult.Data.AccessToken,
                        lockoutOnFailure = false,
                        UserId = refreshTokenResult.Data.UserId
                    };
                    return IdentityResponse<SignInDTO>.CreateSuccessResponse(responseDTO, IdentityResponseMesage.UserSignedIn, System.Net.HttpStatusCode.OK);
                }
            }
            return IdentityResponse<SignInDTO>.CreateErrorResponse(IdentityResponseMesage.UserSignInFailed, System.Net.HttpStatusCode.Unauthorized, new List<string>());
        }

        public async Task<IdentityResult> RegisterAsync(RegistrationDTO registrationDTO)
        {
            var user = new ApplicationUser
            {
                UserName = registrationDTO.userName,
                Email = registrationDTO.email
            };

            var result = await _userManager.CreateAsync(user, registrationDTO.password);
            if (!result.Succeeded)
            {
                
            }
            return result;
        }
        private async Task<string> GenerateToken(ApplicationUser applicationUser)
        {
            var userClaims = await _userManager.GetClaimsAsync(applicationUser);
            var roles = await _userManager.GetRolesAsync(applicationUser);

            var roleClaims = new List<Claim>();
            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                new Claim("uid", applicationUser.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: signingCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
        private async Task<string> CreateRefreshTokenAsync(ApplicationUser user)
        {
            var refreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
            await _userManager.UpdateAsync(user);
            return refreshToken;
        }
        public async Task<IResponseTypes<SignInDTO, IdentityResponseMesage>> RefreshTokenAsync(RefreshTokenDTO refreshDTO)
        {

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshDTO.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return  IdentityResponse<SignInDTO>.CreateErrorResponse(
                    IdentityResponseMesage.InvalidaRefreshToken, System.Net.HttpStatusCode.Unauthorized, new List<string>());
            }


            var newJwtToken = await GenerateToken(user);

            var newRefreshToken = await CreateRefreshTokenAsync(user);

            var responseDTO = new SignInDTO
            {
                userName = user.UserName,
                isPersistent = true,
                AccessToken = newJwtToken,
                RefreshToken = newRefreshToken,
                lockoutOnFailure = false,
                UserId = int.Parse(user.Id)
            };

            return IdentityResponse<SignInDTO>.CreateSuccessResponse(
                responseDTO, IdentityResponseMesage.SuccessRefreshToken, System.Net.HttpStatusCode.OK);
        }

    }
}
