using FluentValidation;
using SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class KingMateLogicRequestValidator : AbstractValidator<KingMateLogicRequestDTO>
    {
        public KingMateLogicRequestValidator()
        {
            RuleFor(r => r.boardStateRequestDTO).SetValidator(new BoardStateRequestDTOValidator());
            RuleFor(r => r.IsTrainingGame).NotNull();
            RuleFor(r => r.isComputerWin).NotNull();
        }
    }
}
