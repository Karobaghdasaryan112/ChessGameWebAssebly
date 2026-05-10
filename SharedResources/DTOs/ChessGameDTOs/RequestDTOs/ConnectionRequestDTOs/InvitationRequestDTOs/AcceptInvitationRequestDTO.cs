using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs
{
    public class AcceptInvitationRequestDTO : RequestDTO
    {
        public PlayEvent PlayEvent { get; set; }
        public Guid inviterUserGuid { get; set; }
        public Guid receiverUserGuid { get; set; }
    }
}
