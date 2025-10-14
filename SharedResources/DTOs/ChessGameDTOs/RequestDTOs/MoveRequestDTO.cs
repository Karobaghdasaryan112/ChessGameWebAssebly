using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs
{
    public class MoveRequestDTO : ICheseGameRequestDTO
    {
        public string Player { get; set; }
        public int GameId { get; set; }
        public Block Block { get; set; }
    }
}
