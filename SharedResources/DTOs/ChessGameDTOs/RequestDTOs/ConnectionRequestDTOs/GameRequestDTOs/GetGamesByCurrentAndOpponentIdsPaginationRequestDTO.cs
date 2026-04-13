using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class GetGamesByCurrentAndOpponentIdsPaginationRequestDTO : RequestDTO
    {
        public Guid OpponentPlayerGuid { get; set; }
        public Guid CurrentPlayerGuid { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }


    }
}
