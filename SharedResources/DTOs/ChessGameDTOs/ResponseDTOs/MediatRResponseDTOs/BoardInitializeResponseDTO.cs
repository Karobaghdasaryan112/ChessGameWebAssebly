using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs
{
    public class BoardInitializeResponseDTO : ICheseGameResponseDTO
    {

        public Board board { get; set; }
        public Guid GameId { get; set; }
    }
}
