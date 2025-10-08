using FluentValidation;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;

namespace IdentityService.Application.Validation
{
    public class RegistrationDTOValidator : AbstractValidator<RegistrationDTO>
    {
        public RegistrationDTOValidator()
        {

            RuleFor(registration => registration.email)
                           .NotEmpty()
                           .WithMessage("Email is required.")
                           .EmailAddress()
                           .WithMessage("Invalid email format.");

            RuleFor(registration => registration.password)
                            .NotEmpty()
                            .WithMessage("Password is required.")
                            .MinimumLength(6)
                            .WithMessage("Password must be at least 6 characters long.");

            RuleFor(registration => registration.confirmPassword)
                            .NotEmpty()
                            .WithMessage("Password is required.")
                            .Equal(registration => registration.password)
                            .WithMessage("Passwords do not match.");

            RuleFor(registration => registration.firstName)
                            .NotEmpty()
                            .WithMessage("FirstName is required.");

            RuleFor(registration => registration.lastName)
                            .NotEmpty()
                            .WithMessage("FirstName is required.");

        }
    }
}
