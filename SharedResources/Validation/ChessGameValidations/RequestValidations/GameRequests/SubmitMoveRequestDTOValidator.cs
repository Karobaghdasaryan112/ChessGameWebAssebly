using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class SubmitMoveRequestDTOValidator : AbstractValidator<SubmitMoveRequestDTO>
    {
        public SubmitMoveRequestDTOValidator()
        {
            RuleFor(x => x.From)
                .NotNull().WithMessage("CurrentPosition is required.")
                .SetValidator(new PositionValidator());

            RuleFor(x => x.To)
                .NotNull().WithMessage("MovePosition is required.")
                .SetValidator(new PositionValidator());

            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage("GameId cannot be empty.");
        }
    }
}
