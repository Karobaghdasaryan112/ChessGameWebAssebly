using SharedResources.Contracts.DTOs;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class GetGamesByCurrentAndOpponentIdsPaginationResponseDTO 
    {
        public List<HistoryGameDTO> AllGamesHistories { get; set; }
    }
}
