using SharedResources.ChessGameResource.Models;

namespace SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs
{
    public class ClickedBlockInformationDTO
    {
        public Guid GameId { get; set; }
        public Block ClieckedBlock { get; set; }
        public MovableAndCutablePositions MovableAndCutablePositions { get; set; }
    }
}
