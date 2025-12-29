using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class GetOptimizedMoveRequestDTOValidator : AbstractValidator<GetOptimizedMoveRequestDTO>
    {
        public GetOptimizedMoveRequestDTOValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty()
                .WithMessage("GameId is required");

            RuleFor(x => x.ChosenColor)
                .IsInEnum()
                .WithMessage("ChosenColor must be a valid enum value");
        }
    }
}
