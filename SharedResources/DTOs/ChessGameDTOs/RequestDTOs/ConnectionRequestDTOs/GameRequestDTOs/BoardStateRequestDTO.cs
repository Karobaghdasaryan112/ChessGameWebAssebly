using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.ChessGameResource.Models;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class BoardStateRequestDTO
    {
        public Guid GameId { get; set; }
        public Position From {  get; set; }
        public Position To { get; set; }
        public string Player {  get; set; }
        public string OpponentConnectionId { get; set; }
        public bool IsKingChecked { get; set; }
        public Position CheckedKingPosition { get; set; }
        public FigureColors OpponentColor { get; set; }
        public IsReady IsReadyToEvent { get; set; }
        public Block CutableFigure { get; set; }
    }
}
