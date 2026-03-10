using SharedResources.ChessGameResource.Models;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class SameFigureRequest
    {
        public Position Selected { get; set; }
        public Position Current { get; set; }
        public Guid GameId { get; set; }
    }
}
