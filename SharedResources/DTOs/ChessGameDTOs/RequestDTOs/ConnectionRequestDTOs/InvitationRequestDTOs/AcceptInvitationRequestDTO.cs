namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs
{
    public class AcceptInvitationRequestDTO
    {
        public Guid inviterUserGuid { get; set; }
        public Guid receiverUserGuid { get; set; }
    }
}
