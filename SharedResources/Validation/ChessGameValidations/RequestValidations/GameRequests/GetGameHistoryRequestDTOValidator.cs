using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class GetGameHistoryRequestDTOValidator : AbstractValidator<GetGameHistoryRequestDTO>
    {
        public GetGameHistoryRequestDTOValidator()
        {
            RuleFor(x => x.GameId)
               .NotEmpty()
               .WithMessage("GameId must not be empty");
        }
    }
}
