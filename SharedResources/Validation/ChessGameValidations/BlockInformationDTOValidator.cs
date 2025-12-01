using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.Validation.ChessGameValidations
{
    public class BlockInformationDTOValidator : AbstractValidator<ClickedBlockInformationDTO>
    {
        public BlockInformationDTOValidator()
        {
            RuleFor(blockInfo => blockInfo.GameId)
                .NotEmpty()
                .WithMessage("GameId cannot be empty.");
            RuleFor(blockInfo => blockInfo.ClickedPosition)
                .NotNull()
                .WithMessage("ClickedPosition cannot be null.");
            RuleFor(blockInfo => blockInfo.MovableAndCutablePositions)
                .NotNull()
                .WithMessage("MovableAndCutablePositions cannot be null.");
        }
    }
}
