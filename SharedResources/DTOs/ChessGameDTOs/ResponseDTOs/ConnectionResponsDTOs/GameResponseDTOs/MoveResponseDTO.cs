using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class MoveResponseDTO : ICheseGameResponseDTO
    {
        public string Player { get; set; }
        public List<Position>? MovableBlocks { get; set; }
        public List<Position>? CutableBlocks { get; set; }
        public Guid GameId { get ; set ; }
    }
}
