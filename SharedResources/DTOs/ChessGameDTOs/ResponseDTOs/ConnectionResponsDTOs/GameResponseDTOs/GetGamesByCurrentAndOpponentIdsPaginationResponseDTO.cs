using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class GetGamesByCurrentAndOpponentIdsPaginationResponseDTO : IResponseDTO
    {
        public List<HistoryGameDTO> AllGamesHistories { get; set; }
    }
}
