using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;

namespace SharedResources.Validation.ChessGameValidations
{
    public class SubmitMoveDTOValidator : AbstractValidator<SubmitMoveRequestDTO>
    {
        public SubmitMoveDTOValidator()
        {
            RuleFor(x => x.GameId)
                 .GreaterThan(0)
                 .WithMessage("GameId must be greater than 0.");

            RuleFor(x => x.Player)
                .NotEmpty()
                .WithMessage("Player is required.")
                .Must(p => !string.IsNullOrWhiteSpace(p))
                .WithMessage("Player cannot be whitespace.");

            RuleFor(x => x.CurrentPosition)
                .NotNull()
                .WithMessage("CurrentPosition is required.")
                .SetValidator(new PositionValidator());

            RuleFor(x => x.MovePosition)
                .NotNull()
                .WithMessage("MovePosition is required.")
                .SetValidator(new PositionValidator());
        }
    }
}
