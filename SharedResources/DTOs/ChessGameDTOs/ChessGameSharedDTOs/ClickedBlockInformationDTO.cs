using SharedResources.ChessGameResource.Models;

namespace SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs
{
    public class ClickedBlockInformationDTO
    {
        public Block ClieckedBlock { get; set; }
        public MovableAndCutablePositions MovableAndCutablePositions { get; set; }
    }
}
