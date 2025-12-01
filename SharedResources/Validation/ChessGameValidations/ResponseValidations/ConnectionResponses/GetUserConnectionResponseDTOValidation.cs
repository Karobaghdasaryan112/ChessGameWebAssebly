using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;

namespace SharedResources.Validation.ChessGameValidations.ResponseValidations.ConnectionResponses
{
    public class GetUserConnectionResponseDTOValidation : AbstractValidator<GetUserConnectionResponseDTO>
    {
        public GetUserConnectionResponseDTOValidation()
        {
            RuleFor(x => x.UserConnectionDTO)
                .NotNull().WithMessage("UserConnectionDTO cannot be null.");

            RuleFor(x => x.UserConnectionDTO)
                .SetValidator(new UserConnectionDTOValidator());
        }
    }
}
