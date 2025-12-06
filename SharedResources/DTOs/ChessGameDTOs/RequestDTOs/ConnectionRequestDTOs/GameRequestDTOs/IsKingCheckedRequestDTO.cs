using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class IsKingCheckedRequestDTO : IRequestDTO
    {
        public Turn ChosenColor { get; set; }
        public Board CurrentBoard { get; set; }
        public Guid GameId { get; set; }
    }
}
