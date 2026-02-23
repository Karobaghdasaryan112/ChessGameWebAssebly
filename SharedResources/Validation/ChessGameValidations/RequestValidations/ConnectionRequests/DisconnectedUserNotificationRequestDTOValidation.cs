using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests
{
    public class DisconnectedUserNotificationRequestDTOValidation : AbstractValidator<DisconnectedUserNotificationRequestDTO>
    {
        public DisconnectedUserNotificationRequestDTOValidation()
        {
            RuleFor(x => x.ConnectionId)
                .NotEmpty().WithMessage("ConnectionId is required.")
                .Must(BeAValidGuid).WithMessage("ConnectionId must be a valid GUID.");
        }
        private bool BeAValidGuid(string connectionId)
        {
            return Guid.TryParse(connectionId, out _);
        }
    }
}
