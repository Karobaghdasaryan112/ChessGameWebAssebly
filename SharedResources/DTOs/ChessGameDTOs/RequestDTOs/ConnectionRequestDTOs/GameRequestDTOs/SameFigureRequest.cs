using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class SameFigureRequest : RequestDTO
    {
        public Position Selected { get; set; }
        public Position Current { get; set; }
        public Guid GameId { get; set; }
    }
}
