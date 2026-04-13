using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class TrainingGameResponseDTO : IResponseDTO
    {
        public string ClientConnectionId { get; set; }
        public Guid GameId { get; set; }
        public Board Board { get; set; }
    }
}
