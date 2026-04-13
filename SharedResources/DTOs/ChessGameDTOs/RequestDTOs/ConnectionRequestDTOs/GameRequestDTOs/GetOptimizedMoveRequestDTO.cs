using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class GetOptimizedMoveRequestDTO : RequestDTO
    {
        public Guid GameId { get; set; }
        public FigureColors ChosenColor { get; set; }
        public bool IsMaximizingPlayer { get; set; }
    }
}
