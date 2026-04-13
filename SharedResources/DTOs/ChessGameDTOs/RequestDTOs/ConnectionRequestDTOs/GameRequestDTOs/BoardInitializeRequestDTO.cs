using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class BoardInitializeRequestDTO : RequestDTO
    {
        public Guid Player1Id { get; set; }
        public Guid Player2Id { get; set; }
    }
}
