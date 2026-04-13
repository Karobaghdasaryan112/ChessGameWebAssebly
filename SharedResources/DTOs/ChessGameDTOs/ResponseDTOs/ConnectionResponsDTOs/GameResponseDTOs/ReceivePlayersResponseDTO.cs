using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class ReceivePlayersResponseDTO : IResponseDTO
    {
        public UserConnectionDTO Player1_UserConnectionDTO { get; set; }
        public UserConnectionDTO Player2_UserConnectionDTO { get; set; }
        public Guid Player1_UserGuId { get; set; }
        public Guid Player2_UserGuid { get; set; }
    }
}
