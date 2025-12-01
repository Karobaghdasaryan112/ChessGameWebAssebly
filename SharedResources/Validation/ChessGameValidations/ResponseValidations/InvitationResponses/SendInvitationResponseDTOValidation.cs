using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;

namespace SharedResources.Validation.ChessGameValidations.ResponseValidations.InvitationResponses
{
    public class SendInvitationResponseDTOValidation : AbstractValidator<SendInvitationsResponseDTO>
    {
        public SendInvitationResponseDTOValidation()
        {
            RuleFor(x => x.InviterUserConnection)
                .NotNull().WithMessage("InviterUserConnection cannot be null.")
                .SetValidator(new UserConnectionDTOValidator());

            RuleFor(x => x.ReceiverUserConnection)
                .NotNull().WithMessage("ReceiverUserConnection cannot be null.")
                .SetValidator(new UserConnectionDTOValidator());

            RuleFor(x => x.InviterUserGuid)
                .NotEmpty().WithMessage("InviterUserGuid must not be empty GUID.");

            RuleFor(x => x.ReceiverUserGuid)
                .NotEmpty().WithMessage("ReceiverUserGuid must not be empty GUID.");
        }
    }
}
