using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;

namespace SharedResources.Validation.ChessGameValidations
{
    public class MoveDTOValidator : AbstractValidator<MoveRequestDTO>
    {
        public MoveDTOValidator()
        {
            RuleFor(x => x.Player)
               .NotEmpty().WithMessage("Player is required.")
               .Must(p => !string.IsNullOrWhiteSpace(p)).WithMessage("Player cannot be whitespace.");

            RuleFor(x => x.GameId)
                .GreaterThan(0).WithMessage("GameId must be greater than 0.");

            RuleFor(x => x.Block)
                .NotNull().WithMessage("Block is required.")
                .SetValidator(new BlockValidator());
        }
    }
}
