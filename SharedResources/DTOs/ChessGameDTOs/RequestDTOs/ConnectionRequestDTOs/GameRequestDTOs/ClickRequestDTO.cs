using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class ClickRequestDTO
    {
        public string Player { get; set; }
        public Guid GameId { get; set; }
        public FigureColors MyColor { get; set; }
        public Position CurrentPosition { get; set; }
        public ClickedBlockInformationDTO PreviusBlockInformationDTO {  get; set; }
        public Position From { get; set; }
        public Position To { get; set; }
    }
}
