using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class MoveResponseDTO : ICheseGameResponseDTO
    {
        public string Player { get; set; }
        public List<Block>? MovableBlocks { get; set; }
        public List<Block>? CutableBlocks { get; set; }
        public Guid GameId { get; set; }
        public IsReady IsReadyToEvent {  get; set; }
    }

}
