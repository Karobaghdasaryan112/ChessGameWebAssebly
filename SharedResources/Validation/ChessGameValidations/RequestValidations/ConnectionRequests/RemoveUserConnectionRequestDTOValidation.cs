using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests
{
    public class RemoveUserConnectionRequestDTOValidation : AbstractValidator<RemoveUserConnectionRequestDTO>
    {
        public RemoveUserConnectionRequestDTOValidation()
        {
            RuleFor(x => x.UserGuid)
                .NotEmpty().WithMessage("UserGuid is required.")
                .Must(g => g != Guid.Empty).WithMessage("UserGuid cannot be empty GUID.");

            RuleFor(x => x.ConnectionId)
                .NotEmpty().WithMessage("ConnectionId is required.");
        }
    }
}
