using SharedResources.Contracts;
using SharedResources.Contracts.DTOs;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class GetAllHistoryWidgetsResponseDTO : IResponseDTO
    {
        public List<HistoryGameDTO> AllGamesHistories { get; set; }
    }
}