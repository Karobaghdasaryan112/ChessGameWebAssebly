using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class SendGameStateResponseDTO : IResponseDTO
    {
        public Board Board { get; set; }
    }
}
