using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs
{
    public class AcceptInvitationResponseDTO : IResponseDTO
    {
        public Guid GameId { get; set; }
        public UserConnectionDTO? PlayerOne_UserConnectionResponseDTO { get; set; }
        public UserConnectionDTO? PlayerTwo_UserConnectionResponseDTO { get; set; }
    }
}
