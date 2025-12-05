using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class GetAllHistoryWidgetRequestDTO : ICheseGameRequestDTO
    {
        public string CurrentPlayer { get; set; }
        public Guid CurrentPlayerId { get; set; }
        public string Result { get; set; }
        public Guid GameId { get; set; }
    }
}
