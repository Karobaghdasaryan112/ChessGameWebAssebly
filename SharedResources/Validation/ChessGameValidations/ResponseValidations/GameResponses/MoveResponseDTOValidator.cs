using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;

namespace SharedResources.Validation.ChessGameValidations.ResponseValidations.GameResponses
{
    public class MoveResponseDTOValidator : AbstractValidator<MoveResponseDTO>
    {
        public MoveResponseDTOValidator()
        {
            RuleFor(x => x.Player)
                .NotEmpty()
                .WithMessage("Player cannot be empty.");

            RuleFor(x => x.GameId)
                .NotEmpty()
                .WithMessage("GameId cannot be empty.");

            RuleFor(x => x.IsReadyToEvent)
                .IsInEnum()
                .WithMessage("IsReadyToEvent must be a valid enum value.");

            RuleForEach(x => x.MovableBlocks)
                .SetValidator(new BlockValidator())
                .When(x => x.MovableBlocks != null);

            RuleForEach(x => x.CutableBlocks)
                .SetValidator(new BlockValidator())
                .When(x => x.CutableBlocks != null);
        }
    }
}
