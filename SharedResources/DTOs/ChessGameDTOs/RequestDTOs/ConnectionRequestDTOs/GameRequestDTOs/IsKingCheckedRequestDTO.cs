using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class IsKingCheckedRequestDTO : RequestDTO
    {
        public Turn ChosenColor { get; set; }
        public Board CurrentBoard { get; set; }
        public Guid GameId { get; set; }
    }
}
