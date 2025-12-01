using SharedResources.ChessGameResource.Models;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class SubmitMoveRequestDTO
    {
        public Guid GameId { get; set; }
        public Position? From { get; set; }
        public Position? To { get; set; }
        public Board CurrentBoardState { get; set; }
    }
}
