using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class BoardInitializeRequestDTOValidation : AbstractValidator<BoardInitializeRequestDTO>
    {
        public BoardInitializeRequestDTOValidation()
        {
            RuleFor(x => x.Player1Id).NotEmpty().WithMessage("Player1Id cannot be empty.");
            RuleFor(x => x.Player2Id).NotEmpty().WithMessage("Player2Id cannot be empty.");

        }
    }
}
