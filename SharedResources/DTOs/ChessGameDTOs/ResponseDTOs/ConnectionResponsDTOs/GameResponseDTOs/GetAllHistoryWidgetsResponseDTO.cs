using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class GetAllHistoryWidgetsResponseDTO : IResponseDTO
    {
        public List<OpponentsHistoryDTO> OpponentHistories { get; set; }
            
    }
}