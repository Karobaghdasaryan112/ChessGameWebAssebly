using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.InvitationRequests
{
    public class AcceptInvitationRequestDTOValidation : AbstractValidator<AcceptInvitationRequestDTO>
    {
        public AcceptInvitationRequestDTOValidation()
        {
            RuleFor(x => x.inviterUserGuid)
                .NotEmpty().WithMessage("InviterUserGuid is required.")
                .Must(g => g != Guid.Empty).WithMessage("InviterUserGuid cannot be empty GUID.");

            RuleFor(x => x.receiverUserGuid)
                .NotEmpty().WithMessage("ReceiverUserGuid is required.")
                .Must(g => g != Guid.Empty).WithMessage("ReceiverUserGuid cannot be empty GUID.");
        }
    }
}
