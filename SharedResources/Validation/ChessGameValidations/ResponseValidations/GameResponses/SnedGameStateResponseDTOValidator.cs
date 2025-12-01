using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;

namespace SharedResources.Validation.ChessGameValidations.ResponseValidations.GameResponses
{
    public class SnedGameStateResponseDTOValidator : AbstractValidator<SendGameStateResponseDTO>
    {
        public SnedGameStateResponseDTOValidator()
        {
            RuleFor(x => x.Board)
                .NotNull().WithMessage("Board cannot be null.");
        }
    }
}
