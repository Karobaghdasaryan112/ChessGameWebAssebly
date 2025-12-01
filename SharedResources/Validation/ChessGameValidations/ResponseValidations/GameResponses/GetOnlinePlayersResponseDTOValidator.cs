using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;

namespace SharedResources.Validation.ChessGameValidations.ResponseValidations.GameResponses
{
    public class GetOnlinePlayersResponseDTOValidator : AbstractValidator<GetOnlinePlayersResponseDTO>
    {
        public GetOnlinePlayersResponseDTOValidator()
        {
            RuleFor(x => x.OnlinePlayers)
                .NotNull().WithMessage("OnlinePlayers cannot be null.")
                .Must(dict => dict.Count > 0)
                .WithMessage("OnlinePlayers must contain at least one player.");

            RuleForEach(x => x.OnlinePlayers)
                .Must(pair => pair.Key != Guid.Empty)
                .WithMessage("PlayerId (dictionary key) cannot be empty.")
                .Must(pair => pair.Value != null)
                .WithMessage("UserConnectionDTO cannot be null.");
        }
    }
}
