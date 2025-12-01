using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.InvitationRequests
{
    public class SendInvitationRequestDTOValidation : AbstractValidator<SendInvitationRequestDTO>
    {
        public SendInvitationRequestDTOValidation()
        {
            RuleFor(x => x.InviterUserConnection)
                .NotNull().WithMessage("InviterUserConnection cannot be null.")
                .SetValidator(new UserConnectionDTOValidator());

            RuleFor(x => x.ReceiverUserConnection)
                .NotNull().WithMessage("ReceiverUserConnection cannot be null.")
                .SetValidator(new UserConnectionDTOValidator());

            RuleFor(x => x.InviterPlayerId)
                .NotEmpty().WithMessage("InviterPlayerId is required.")
                .Must(g => g != Guid.Empty).WithMessage("InviterPlayerId cannot be empty GUID.");

            RuleFor(x => x.ReceiverPlayerId)
                .NotEmpty().WithMessage("ReceiverPlayerId is required.")
                .Must(g => g != Guid.Empty).WithMessage("ReceiverPlayerId cannot be empty GUID.");
        }
    }
}
