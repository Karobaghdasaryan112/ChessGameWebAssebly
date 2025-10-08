using FluentValidation;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;

namespace IdentityService.Application.Validation
{
    public class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        public LoginDTOValidator()
        {

            RuleFor(login => login.password)
                .NotEmpty()
                .WithMessage("Password is required.");

            RuleFor(login => login.email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

        }
    }
}
