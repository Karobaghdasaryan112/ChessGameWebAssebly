using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs
{
    public class RemoveUserConnectionRequestDTO : RequestDTO
    {
        //this two fields for 2 methods 
        public Guid UserGuid { get; set; }
        public string ConnectionId { get; set; }
    }
}
