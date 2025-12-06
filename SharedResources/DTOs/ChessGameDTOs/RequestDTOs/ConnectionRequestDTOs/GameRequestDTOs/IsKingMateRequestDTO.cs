using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class IsKingMateRequestDTO : IRequestDTO
    {
        public Board? CurrentBoard;
        public Guid GameId;
        public Turn ChosenColor;
    }
}
