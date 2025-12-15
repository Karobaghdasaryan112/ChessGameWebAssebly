using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class GetAllHistoryWidgetRequestDTO : IRequestDTO 
    {
        public Guid CurrentPlayerId { get; set; }
    }
}
