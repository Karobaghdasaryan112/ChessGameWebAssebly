using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class GetOnlinePlayersRequestDTOValidator : AbstractValidator<GetONlinePlayersRequestDTO>
    {
        public GetOnlinePlayersRequestDTOValidator()
        {
            RuleFor(request => request.UserGuid)
                .NotEmpty()
                .WithMessage("UserGuid cannot be empty.");
            RuleFor(request => request.UserGuid)
                .Must(guid => guid != Guid.Empty)
                .WithMessage("UserGuid must be a valid GUID.");
            RuleFor(request => request.UserGuid)
                .NotNull()
                .WithMessage("UserGuid cannot be null.");

        }
    }
}
