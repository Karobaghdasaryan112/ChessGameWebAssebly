using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs
{
    public class RemoveUserConnectionRequestDTO : RequestDTO
    {
        public Guid UserGuid { get; set; }
        public string ConnectionId { get; set; }
        public Guid GameId { get; set; }
    }
}
