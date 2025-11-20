using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs
{
    public class MoveRequestDTO : ICheseGameRequestDTO
    {
        public string Player { get; set; }
        public Position From { get; set; }
        public Position To { get; set; }
        public FigureColors MyColor { get; set; }
        public Guid GameId { get; set; }
    }
}
