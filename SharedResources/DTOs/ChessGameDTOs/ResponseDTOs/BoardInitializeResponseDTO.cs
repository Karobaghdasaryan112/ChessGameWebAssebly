using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs
{
    public class BoardInitializeResponseDTO : ICheseGameResponseDTO
    {
        public int GameId { get; set; }
        public Board board { get; set; }
    }
}
