using SharedResources.Contracts.DTOs;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs
{
    public class InvitationResponseDTO : ICheseGameResponseDTO
    {
        public Guid GameId { get; set; }
        public UserConnectionDTO? PlayerOne_UserConnectionResponseDTO { get; set; }
        public UserConnectionDTO? PlayerTwo_UserConnectionResponseDTO { get; set; }
    }
}
