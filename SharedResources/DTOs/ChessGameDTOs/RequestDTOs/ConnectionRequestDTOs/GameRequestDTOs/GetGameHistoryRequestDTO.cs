using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class GetGameHistoryRequestDTO : IRequestDTO
    {
        public Guid GameId { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
    }
}
