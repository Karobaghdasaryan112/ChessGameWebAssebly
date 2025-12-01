using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;

namespace SharedResources.Validation.ChessGameValidations.ResponseValidations.GameResponses
{
    public class ReceivePlayersResponseDTOValidator : AbstractValidator<ReceivePlayersResponseDTO>
    {
        public ReceivePlayersResponseDTOValidator()
        {
            RuleFor(x => x.Player1_UserConnectionDTO)
                .NotNull()
                .WithMessage("Player1_UserConnectionDTO cannot be null.")
                .SetValidator(new UserConnectionDTOValidator());

            RuleFor(x => x.Player2_UserConnectionDTO)
                .NotNull()
                .WithMessage("Player2_UserConnectionDTO cannot be null.")
                .SetValidator(new UserConnectionDTOValidator());

            RuleFor(x => x.Player1_UserGuId)
                .NotEmpty()
                .WithMessage("Player1_UserGuId cannot be empty.");

            RuleFor(x => x.Player2_UserGuid)
                .NotEmpty()
                .WithMessage("Player2_UserGuid cannot be empty.");
        }
    }
}
