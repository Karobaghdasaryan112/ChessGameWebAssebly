using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class IsKingMateRequestDTOValidation : AbstractValidator<IsKingMateRequestDTO>
    {
        public IsKingMateRequestDTOValidation()
        {
            RuleFor(x => x.GameId)
                .NotEmpty()
                .WithMessage("GameId is required.");

            RuleFor(x => x.ChosenColor)
                .NotNull()
                .IsInEnum()
                .WithMessage("ChosenColor must be a valid Turn value (White or Black).");

            RuleFor(x => x.CurrentBoard)
                .NotNull()
                .WithMessage("CurrentBoard is required.");
        }
    }
}
