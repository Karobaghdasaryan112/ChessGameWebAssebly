using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.ChessGameResource.Models;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class BoardStateResponseDTO
    {
        public Position? From { get; set; }
        public Guid GameId { get; set; }
        public Position? To { get; set; }
        public Block? CutableFigure { get; set; }
        public string? OpponentConnectionId { get; set; }
        public string Player {  get; set; }
        public bool IsKingChecked { get; set; }
        public bool IsKingMate { get; set; }
        public bool IsMyConnection { get; set; }
        public bool Win { get; set; }

        // Position of the king of the player receiving the event
        // Used to determine if the king is in check after a move
        public Position? KingPosition { get; set; }
       
        public IsReady IsReadyToEvent {  get; set; }
        public FigureColors OpponentColor { get; set; }
    }
}
