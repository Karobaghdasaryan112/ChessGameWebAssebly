using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class SendGameStateRequestDTOValidator : AbstractValidator<SendGameStateReqeustDTO>
    {
        public SendGameStateRequestDTOValidator()
        {
            RuleFor(req => req.GameId)
                .NotEmpty()
                .WithMessage("GameId cannot be empty.");
        }
    }
}
