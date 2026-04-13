using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs
{
    public class AddUserConnectionRequestDTO : RequestDTO
    {
        public Guid userGuid { get; set; }
       public UserConnectionDTO userConnection { get; set; }
    }
}
