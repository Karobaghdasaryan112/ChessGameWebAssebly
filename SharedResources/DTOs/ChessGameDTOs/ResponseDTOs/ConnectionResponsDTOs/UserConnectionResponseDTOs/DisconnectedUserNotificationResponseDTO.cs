using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs
{
    public class DisconnectedUserNotificationResponseDTO
    {
        public bool IsUserDisconnectedSuccess { get; set; }
        public UserConnectionDTO ActiveGame { get; set; }

    }
}
