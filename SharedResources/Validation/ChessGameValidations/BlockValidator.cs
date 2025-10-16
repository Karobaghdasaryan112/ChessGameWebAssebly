using FluentValidation;
using SharedResources.ChessGameResource.Models;

namespace SharedResources.Validation.ChessGameValidations
{
    public class BlockValidator : AbstractValidator<Block>
    {
        public BlockValidator()
        {
            RuleFor(x => x.Position)
              .NotNull().WithMessage("Position is required.")
              .SetValidator(new PositionValidator());

            RuleFor(x => x.BlockColor)
               .IsInEnum()
               .WithMessage("BlockColor must be either White or Black.");
        }
    }
}
