using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class BoardStateRequestDTO : IRequestDTO
    {
        public bool IsOpponentComputer { get; set; }
        public Guid GameId { get; set; }
        public Position? From {  get; set; }
        public Position? To { get; set; }
        public string Player {  get; set; }
        public bool IsKingChecked { get; set; }
        public bool IsKingMate { get; set; }
        public Position CheckedKingPosition { get; set; }
        public FigureColors OpponentColor { get; set; }
        public IsReady IsReadyToEvent { get; set; }
        public Block CutableFigure { get; set; }
        public Board GameState { get; set; }
    }
}
