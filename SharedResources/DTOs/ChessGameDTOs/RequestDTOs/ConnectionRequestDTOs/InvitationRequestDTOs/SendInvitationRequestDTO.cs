using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs
{
    public class SendInvitationRequestDTO
    {
        public UserConnectionDTO InviterUserConnection { get; set; }
        public UserConnectionDTO ReceiverUserConnection { get; set; }
        public Guid InviterPlayerId;
        public Guid ReceiverPlayerId;
    }
}
