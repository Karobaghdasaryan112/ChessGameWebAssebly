using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class BoardStateResponseDTO
    {
        public Position From { get; set; }
        public Guid GameId { get; set; }
        public Position To { get; set; }
        public Block CutableFigure { get; set; }
        public string OpponentConnectionId { get; set; }
        public FigureColors OpponentColor { get; set; }
    }
}
