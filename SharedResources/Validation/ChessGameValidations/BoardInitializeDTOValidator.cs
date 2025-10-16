using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;

namespace SharedResources.Validation.ChessGameValidations
{
    public class BoardInitializeDTOValidator : AbstractValidator<BoardInitializeRequestDTO>
    {
        public BoardInitializeDTOValidator()
        {
            RuleFor(x => x.Player1Id)
                .NotEmpty()
                .WithMessage("Player1Id is required.");

            RuleFor(x => x.Player2Id)
                .NotEmpty()
                .WithMessage("Player2Id is required.");

            RuleFor(x => x)
                .Must(dto => dto.Player1Id != dto.Player2Id)
                .WithMessage("Player1Id and Player2Id cannot be the same.");
        }
    }
}
