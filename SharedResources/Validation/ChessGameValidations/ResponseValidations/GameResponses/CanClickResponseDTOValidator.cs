using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;

namespace SharedResources.Validation.ChessGameValidations.ResponseValidations.GameResponses
{
    public class CanClickResponseDTOValidator : AbstractValidator<CanClickResponseDTO>
    {
        public CanClickResponseDTOValidator()
        {
            RuleFor(x => x.ClickedBlock)
                .NotNull().WithMessage("ClickedBlock cannot be null.")
                .SetValidator(new BlockValidator());
        }
    }
}
