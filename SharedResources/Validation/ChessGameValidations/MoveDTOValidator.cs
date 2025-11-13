using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations
{
    public class MoveDTOValidator : AbstractValidator<MoveRequestDTO>
    {
        public MoveDTOValidator()
        {
            RuleFor(x => x.Player)
               .NotEmpty().WithMessage("Player is required.")
               .Must(p => !string.IsNullOrWhiteSpace(p)).WithMessage("Player cannot be whitespace.");


            RuleFor(x => x.Block)
                .NotNull().WithMessage("Block is required.")
                .SetValidator(new BlockValidator());
        }
    }
}
