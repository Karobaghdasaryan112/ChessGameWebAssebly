using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;

public class SameFigureRequestDTOValidator : AbstractValidator<SameFigureRequest>
{
    public SameFigureRequestDTOValidator()
    {
        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage("GameId is required.");

        RuleFor(x => x.Selected)
            .NotNull().WithMessage("Selected position is required.");

        RuleFor(x => x.Current)
            .NotNull().WithMessage("Current position is required.");

        RuleFor(x => x)
            .Must(x => !x.Selected.Equals(x.Current))
            .WithMessage("Selected and Current positions must be different.");
    }
}