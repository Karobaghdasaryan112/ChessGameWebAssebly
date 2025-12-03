using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class BoardStateRequestDTOValidator : AbstractValidator<BoardStateRequestDTO>
    {
        public BoardStateRequestDTOValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage("GameId is required.");
            RuleFor(x => x.From)
                .NotNull().When(x => !x.IsKingChecked).WithMessage("From position is required.");
            RuleFor(x => x.To)
                .NotNull().When(x => !x.IsKingChecked).WithMessage("To position is required.");
            RuleFor(x => x.Player)
                .NotEmpty().WithMessage("Player is required.");
            RuleFor(x => x.CheckedKingPosition)
                .NotNull().When(x => x.IsKingChecked)
                .WithMessage("CheckedKingPosition is required when IsKingChecked is true.");
            RuleFor(x => x.OpponentColor)
                .IsInEnum().WithMessage("OpponentColor must be a valid FigureColors value.");
            RuleFor(x => x.IsReadyToEvent)
                .IsInEnum().WithMessage("IsReadyToEvent must be a valid IsReady value.");
        }
    }
}
