using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs
{
    public class InvitationResponseDTO : ICheseGameResponseDTO
    {
        public Guid GameId { get; set; }
        public UserConnectionResponseDTO? PlayerOne_UserConnectionResponseDTO { get; set; }
        public UserConnectionResponseDTO? PlayerTwo_UserConnectionResponseDTO { get; set; }
    }
}
