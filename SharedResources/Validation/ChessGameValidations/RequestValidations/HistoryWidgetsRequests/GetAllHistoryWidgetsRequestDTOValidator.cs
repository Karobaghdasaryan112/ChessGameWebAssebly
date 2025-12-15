using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.HistoryWidgetsRequests
{
    public class GetAllHistoryWidgetsRequestDTOValidator : AbstractValidator<GetAllHistoryWidgetRequestDTO>
    {
        public GetAllHistoryWidgetsRequestDTOValidator()
        {
            //RuleFor(x => x.CurrentPlayer)
            //    .NotEmpty().WithMessage("CurrentPlayer is required.")
            //    .MaximumLength(50).WithMessage("CurrentPlayer must be under 50 characters.");

            //RuleFor(x => x.Result)
            //    .NotEmpty().WithMessage("Result is required.")
            //    .Must(result => result == "Win" || result == "Lose" || result == "Draw")
            //    .WithMessage("Result must be one of: Win, Lose, Draw.");

            //RuleFor(x => x.GameId)
            //    .NotEmpty().WithMessage("GameId is required.")
            //    .NotEqual(Guid.Empty).WithMessage("GameId must be a valid GUID.");
        }
    }
}
