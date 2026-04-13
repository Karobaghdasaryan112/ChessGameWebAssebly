using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class IsKingMateRequestDTO : RequestDTO
    {
        public Board? CurrentBoard;
        public Guid GameId;
        public Turn ChosenColor;
    }
}
