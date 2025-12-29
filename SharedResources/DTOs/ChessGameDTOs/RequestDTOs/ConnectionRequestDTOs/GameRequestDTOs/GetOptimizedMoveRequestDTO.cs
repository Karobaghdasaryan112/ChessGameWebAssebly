using SharedResources.ChessGameResource.Enums.Colors;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class GetOptimizedMoveRequestDTO
    {
        public Guid GameId { get; set; }
        public FigureColors ChosenColor { get; set; }
        public bool IsMaximizingPlayer { get; set; }
    }
}
