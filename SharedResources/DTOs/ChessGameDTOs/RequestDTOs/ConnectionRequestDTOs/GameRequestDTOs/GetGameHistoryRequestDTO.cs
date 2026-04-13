using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class GetGameHistoryRequestDTO : RequestDTO
    {
        public Guid GameId { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
    }
}
