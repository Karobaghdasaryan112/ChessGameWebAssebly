using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.HistoryWidgetsRequests
{
    public class GetGamesByCurrentAndOpponentIdsPaginationRequestDTOValidator : AbstractValidator<GetGamesByCurrentAndOpponentIdsPaginationRequestDTO>
    {
        public GetGamesByCurrentAndOpponentIdsPaginationRequestDTOValidator()
        {
            RuleFor(x => x.OpponentPlayerGuid).NotEmpty();
            RuleFor(x => x.CurrentPlayerGuid).NotEmpty();
            RuleFor(x => x.CurrentPage).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(0);
        }
    }
}
