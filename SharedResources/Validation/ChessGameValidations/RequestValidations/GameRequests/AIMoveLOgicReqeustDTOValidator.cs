using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class AIMoveLOgicReqeustDTOValidator : AbstractValidator<AIMoveLogicRequestDTO>
    {
        public AIMoveLOgicReqeustDTOValidator()
        {
            RuleFor(r => r.BoardRequestDTO).SetValidator(new BoardStateRequestDTOValidator());
            
        }
    }
}
