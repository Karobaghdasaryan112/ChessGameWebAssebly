using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs
{
    public class MoveResponseDTO : ICheseGameResponseDTO
    {
        public int GameId { get; set; }
        public string Player { get; set; }
        public List<Board>? MovableBlocks { get; set; }
        public List<Board>? CutableBlocks { get; set; }

    }
}
