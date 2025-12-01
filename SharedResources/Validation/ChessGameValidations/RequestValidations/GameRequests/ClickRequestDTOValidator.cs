using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class ClickRequestDTOValidator : AbstractValidator<ClickRequestDTO>
    {
        public ClickRequestDTOValidator()
        {
            RuleFor(clickRequest => clickRequest.Player)
                .NotEmpty()
                .WithMessage("Player cannot be empty.");
            RuleFor(clickRequest => clickRequest.GameId)
                .NotEmpty()
                .WithMessage("GameId cannot be empty.");
            RuleFor(clickRequest => clickRequest.CurrentPosition)
                .NotNull()
                .WithMessage("CurrentPosition cannot be null.");
            RuleFor(clickRequest => clickRequest.PreviusBlockInformationDTO)
                .NotNull()
                .WithMessage("PreviusBlockInformationDTO cannot be null.");
            RuleFor(clickRequest => clickRequest.From)
                .NotNull()
                .WithMessage("From position cannot be null.");
            //RuleFor(clickRequest => clickRequest.To)
            //    .NotNull()
            //    .WithMessage("To position cannot be null.");
        }
    }
}
