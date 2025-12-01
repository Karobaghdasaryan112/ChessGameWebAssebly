using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;

namespace SharedResources.Validation.ChessGameValidations.ResponseValidations.GameResponses
{
    public class SubmitMoveResponseDTOValidator : AbstractValidator<SubmitMoveResponseDTO>
    {
        public SubmitMoveResponseDTOValidator()
        {
            RuleFor(x => x.IsKingChecked)
                .NotNull().WithMessage("IsKingChecked must be provided.");

            RuleFor(x => x.IsKingMate)
                .NotNull().WithMessage("IsKingMate must be provided.");

            RuleFor(x => x.IsMoveSuccess)
                .NotNull().WithMessage("IsMoveSuccess must be provided.");
        }
    }
}
