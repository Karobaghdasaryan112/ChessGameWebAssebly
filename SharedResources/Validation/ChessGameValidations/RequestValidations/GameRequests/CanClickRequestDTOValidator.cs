using FluentValidation;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class CanClickRequestDTOValidator : AbstractValidator<CanClickRequestDTO>
    {
        public CanClickRequestDTOValidator()
        {
            RuleFor(canClick => canClick.ClickedBlockInformationDto)
                .NotNull()
                .WithMessage("ClickedBlockInformationDto cannot be null.")
                .SetValidator(new BlockInformationDTOValidator());

            RuleFor(canClick => canClick.FigureColor)
                .Must(color => color != FigureColors.None)
                .WithMessage("FigureColor must be either White or Black.");

            RuleFor(canClick => canClick.CurrentBlock)
                .NotNull()
                .WithMessage("CurrentBlock cannot be null.");

            RuleFor(canClick => canClick.CurrentBoardBoardState)
                .NotNull()
                .WithMessage("CurrentBoardBoardState cannot be null.");
        }
    }
}
