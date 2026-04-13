using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs
{
    public class SendInvitationRequestDTO : RequestDTO
    {
        public UserConnectionDTO InviterUserConnection { get; set; }
        public UserConnectionDTO ReceiverUserConnection { get; set; }
        //this is for continue the game
        public Guid GameId { get; set; }
        public Guid InviterPlayerId { get; set; }
        public Guid ReceiverPlayerId { get; set; }
    }
}
