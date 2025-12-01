using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests
{
    public class GetUserConnectionRequestDTOValidation : AbstractValidator<GetUserConnectionRequestDTO>
    {
        public GetUserConnectionRequestDTOValidation()
        {
            RuleFor(x => x.UserGuid)
                .NotEmpty().WithMessage("UserGuid is required.")
                .Must(g => g != Guid.Empty).WithMessage("UserGuid cannot be an empty GUID.");
        }
    }
}
