using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class IsKingCheckedRequestDTOValidation : AbstractValidator<IsKingCheckedRequestDTO>
    {
        public IsKingCheckedRequestDTOValidation()
        {
            RuleFor(x => x.ChosenColor)
                .NotNull()
                .WithMessage("ChosenColor is required.")
                .IsInEnum()
                .WithMessage("ChosenColor must be a valid Turn value (typically White or Black).");

            RuleFor(x => x.CurrentBoard)
                .NotNull()
                .WithMessage("CurrentBoard is required.");
        }
    }
}
