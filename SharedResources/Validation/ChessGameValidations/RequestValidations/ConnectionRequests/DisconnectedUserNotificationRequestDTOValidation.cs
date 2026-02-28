using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests
{
    public class DisconnectedUserNotificationRequestDTOValidation : AbstractValidator<DisconnectedUserNotificationRequestDTO>
    {
        public DisconnectedUserNotificationRequestDTOValidation()
        {
            RuleFor(x => x.ConnectionId)
                .NotEmpty().WithMessage("ConnectionId is required.");
        }
    }
}
