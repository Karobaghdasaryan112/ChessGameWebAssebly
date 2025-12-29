using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class GetOptimizedMoveResponseDTO : IResponseDTO
    {
        public Position? FromPosition { get; set; }
        public Position? ToPosition { get; set; }
        public Guid GameId { get; set; }
    }
}
