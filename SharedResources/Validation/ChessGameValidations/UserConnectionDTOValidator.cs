using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.Validation.ChessGameValidations
{
    public class UserConnectionDTOValidator : AbstractValidator<UserConnectionDTO>
    {
        public UserConnectionDTOValidator()
        {
            RuleFor(dto => dto.UserName)
                .NotEmpty()
                .WithMessage("UserId cannot be empty.");
            RuleFor(dto => dto.ConnectionId)
                .NotEmpty()
                .WithMessage("ConnectionId cannot be empty.");

        }
    }
}
