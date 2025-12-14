using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class GetAllHistoryWidgetRequestDTO : IResponseDTO
    {
        public Guid CurrentPlayerId { get; set; }
    }
}
