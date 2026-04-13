using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs
{
    public class GetUserConnectionRequestDTO : RequestDTO
    {
        public Guid UserGuid {  get; set; }
    }
}
