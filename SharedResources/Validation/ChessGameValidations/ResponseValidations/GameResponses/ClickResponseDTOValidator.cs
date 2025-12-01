using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;

namespace SharedResources.Validation.ChessGameValidations.ResponseValidations.GameResponses
{
    public class ClickResponseDTOValidator : AbstractValidator<ClickResponseDTO>
    {
        public ClickResponseDTOValidator()
        {
            RuleFor(x => x.Player)
                .NotEmpty().WithMessage("Player cannot be empty.");

            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage("GameId cannot be empty.");

            RuleForEach(x => x.MovableBlocks)
                .SetValidator(new BlockValidator())
                .When(x => x.MovableBlocks != null);

            RuleForEach(x => x.CutableBlocks)
                .SetValidator(new BlockValidator())
                .When(x => x.CutableBlocks != null);
        }
    }
}
