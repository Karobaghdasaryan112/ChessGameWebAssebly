using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.DTOs;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs
{
    public class MoveRequestDTO : ICheseGameRequestDTO
    {
        public string Player { get; set; }
        public Guid CurrentPlayerId { get; set; }
        public Position From { get; set; }
        public Position To { get; set; }
        public Block CurrentBlock { get; set; }
        public ClickedBlockInformationDTO PreviusBlockInformationDTO { get; set; }
        public FigureColors MyColor { get; set; }
        public Guid GameId { get; set; }
    }
}
