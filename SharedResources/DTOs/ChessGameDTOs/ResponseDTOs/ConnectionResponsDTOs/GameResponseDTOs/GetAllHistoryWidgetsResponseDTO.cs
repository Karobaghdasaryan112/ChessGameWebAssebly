using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class GetAllHistoryWidgetsResponseDTO
    {
        public List<HistoryGameDTO> AllGamesHistories { get; set; }
    }
}