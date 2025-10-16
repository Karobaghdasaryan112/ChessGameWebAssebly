using FluentValidation;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;

namespace SharedResources.Validation.IdentityValidations
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
