using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class ClickResponseDTO
    {
        public string Player { get; set; }
        public List<Block>? MovableBlocks { get; set; }
        public List<Block>? CutableBlocks { get; set; }
        public List<CastlingInfosDTO> CastlingInfosDTOs { get; set; }
        public Guid GameId { get; set; }
    }
}
