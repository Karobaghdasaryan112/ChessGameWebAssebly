using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs
{
    public class GetUserConnectionResponseDTO : IResponseDTO
    {
        public UserConnectionDTO UserConnectionDTO { get; set; }
    }
}
