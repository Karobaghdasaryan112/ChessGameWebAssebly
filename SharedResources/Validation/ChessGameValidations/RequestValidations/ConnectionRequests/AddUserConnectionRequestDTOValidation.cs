using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests
{
    public class AddUserConnectionRequestDTOValidation : AbstractValidator<AddUserConnectionRequestDTO>
    {
        public AddUserConnectionRequestDTOValidation()
        {
            RuleFor(x => x.userGuid)
                .NotEmpty().WithMessage("UserGuid is required.")
                .Must(g => g != Guid.Empty).WithMessage("UserGuid cannot be empty GUID.");

            RuleFor(x => x.userConnection)
                .NotNull().WithMessage("UserConnection cannot be null.");

            RuleFor(x => x.userConnection)
                .SetValidator(new UserConnectionDTOValidator());
        }
    }
}
