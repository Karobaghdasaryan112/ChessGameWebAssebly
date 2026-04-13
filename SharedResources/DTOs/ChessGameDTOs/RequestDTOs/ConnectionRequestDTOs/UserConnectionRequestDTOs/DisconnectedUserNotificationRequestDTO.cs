using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs
{
    public class DisconnectedUserNotificationRequestDTO : RequestDTO
    {
        public string ConnectionId { get; set; }
    }
}
