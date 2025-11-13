using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class MoveResponseDTO : ICheseGameResponseDTO
    {
        public string Player { get; set; }
        public List<Board>? MovableBlocks { get; set; }
        public List<Board>? CutableBlocks { get; set; }
        public Guid GameId { get ; set ; }
    }
}
