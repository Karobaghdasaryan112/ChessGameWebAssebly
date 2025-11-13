using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs
{
    public class AddUserConnectionRequestDTO 
    {
        public Guid userGuid { get; set; }
       public UserConnectionDTO userConnection { get; set; }
    }
}
