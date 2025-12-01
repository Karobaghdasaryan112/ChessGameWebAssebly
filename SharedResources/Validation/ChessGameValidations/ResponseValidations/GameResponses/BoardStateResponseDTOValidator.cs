using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;

namespace SharedResources.Validation.ChessGameValidations.ResponseValidations.GameResponses
{
    public class BoardStateResponseDTOValidator : AbstractValidator<BoardStateResponseDTO>
    {
        public BoardStateResponseDTOValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage("GameId cannot be empty.");

            RuleFor(x => x.Player)
                .NotEmpty().WithMessage("Player is required.");

            RuleFor(x => x.OpponentColor)
                .IsInEnum().WithMessage("OpponentColor is invalid.");

            RuleFor(x => x.IsReadyToEvent)
                .IsInEnum().WithMessage("IsReadyToEvent is invalid.");

            // Validate From position (optional, only when provided)
            When(x => x.From != null, () =>
            {
                RuleFor(x => x.From!).SetValidator(new PositionValidator());
            });

            // Validate To position (optional, only when provided)
            When(x => x.To != null, () =>
            {
                RuleFor(x => x.To!).SetValidator(new PositionValidator());
            });

            // Validate KingPosition (optional, only when provided)
            When(x => x.KingPosition != null, () =>
            {
                RuleFor(x => x.KingPosition!).SetValidator(new PositionValidator());
            });

            // Block validation (optional)
            When(x => x.CutableFigure != null, () =>
            {
                RuleFor(x => x.CutableFigure!)
                    .SetValidator(new BlockValidator());
            });

            // OpponentConnectionId can be null, but if provided – must not be empty
            When(x => x.OpponentConnectionId != null, () =>
            {
                RuleFor(x => x.OpponentConnectionId)
                    .NotEmpty().WithMessage("OpponentConnectionId cannot be an empty string.");
            });

        }
    }
}
