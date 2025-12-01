using FluentValidation;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class MoveRequestDTOValidator : AbstractValidator<MoveRequestDTO>
    {
        public MoveRequestDTOValidator()
        {
            RuleFor(move => move.GameId)
                .NotEmpty()
                .WithMessage("GameId cannot be empty.");

            RuleFor(move => move.Player)
                .NotEmpty()
                .WithMessage("Player cannot be empty.");

            RuleFor(move => move.CurrentPlayerId)
                .NotEmpty()
                .WithMessage("CurrentPlayerId cannot be empty.");

            RuleFor(move => move.From)
                .NotNull()
                .WithMessage("From position cannot be null.");

            RuleFor(move => move.To)
                .NotNull()
                .WithMessage("To position cannot be null.");

            RuleFor(move => move.CurrentPosition)
                .NotNull()
                .WithMessage("CurrentPosition cannot be null.");

            RuleFor(move => move.MyColor)
                .Must(c => c == FigureColors.White || c == FigureColors.Black)
                .WithMessage("MyColor must be either White or Black.");

        }
    }
}
