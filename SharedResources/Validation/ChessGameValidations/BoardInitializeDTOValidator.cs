using FluentValidation;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;

namespace SharedResources.Validation.ChessGameValidations
{
    public class BoardInitializeDTOValidator : AbstractValidator<BoardInitializeRequestDTO>
    {
        public BoardInitializeDTOValidator()
        {
            RuleFor(x => x.MyFigureColor)
                .IsInEnum()
                .WithMessage("MyFigureColor must be a valid enum value (e.g., White or Black).");

            RuleFor(x => x.MyFigureColor)
                .Must(color => color == FigureColors.White || color == FigureColors.Black)
                .WithMessage("Only White or Black colors are allowed.");
        }
    }
}
