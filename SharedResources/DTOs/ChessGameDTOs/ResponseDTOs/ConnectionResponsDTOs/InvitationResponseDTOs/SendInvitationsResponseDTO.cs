using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs
{
    public class SendInvitationsResponseDTO : IResponseDTO
    {
        public UserConnectionDTO InviterUserConnection { get; set; }    
        public UserConnectionDTO ReceiverUserConnection { get; set; }    
        public Guid InviterUserGuid { get; set; }
        public Guid ReceiverUserGuid { get; set; }
        public PlayEvent PlayEvent { get; set; }
    }
}
