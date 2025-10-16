using FluentValidation;
using SharedResources.ChessGameResource.Enums.CriticalValues;
using SharedResources.ChessGameResource.Models;

namespace SharedResources.Validation.ChessGameValidations
{
    public class PositionValidator : AbstractValidator<Position>
    {
        public PositionValidator()
        {
            RuleFor(p => (int)p.VerticalOrientation)
               .InclusiveBetween((int)CriticalPositions.lowCriticalValue, (int)CriticalPositions.highCriticalValue)
               .WithMessage("VerticalOrientation must be between 0 and 7.");

            RuleFor(p => (int)p.HorizontalOrientation)
                .InclusiveBetween((int)CriticalPositions.lowCriticalValue, (int)CriticalPositions.highCriticalValue)
                .WithMessage("HorizontalOrientation must be between 0 and 7.");
        }
    }
}
