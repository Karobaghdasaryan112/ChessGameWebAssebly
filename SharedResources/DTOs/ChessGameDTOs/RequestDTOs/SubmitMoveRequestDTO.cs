using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs
{
    public class SubmitMoveRequestDTO : ICheseGameRequestDTO
    {
        public Position CurrentPosition { get; set; }
        public Position MovePosition { get; set; }
        public string Player { get; set; }
        public Guid GameId { get; set; }
    }
}
