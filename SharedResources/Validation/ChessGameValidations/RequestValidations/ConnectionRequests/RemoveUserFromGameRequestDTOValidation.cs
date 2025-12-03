using FluentValidation;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests
{
    public class RemoveUserFromGameRequestDTOValidation : AbstractValidator<RemoveUserFromGameRequestDTO>
    {
        public RemoveUserFromGameRequestDTOValidation()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage("GameId is required.")
                .NotNull().WithMessage("GameId cannot be null.");
        }
    }
}
