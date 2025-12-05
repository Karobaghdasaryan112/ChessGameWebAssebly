using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class GetGamesByCurrentAndOpponentIdsPaginationRequestDTO : IRequestDTO
    {
        public Guid OpponentPlayerGuid { get; set; }
        public Guid CurrentPlayerGuid { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }


    }
}
