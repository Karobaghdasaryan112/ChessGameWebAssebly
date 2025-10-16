using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs
{
    public class BoardInitializeRequestDTO : ICheseGameRequestDTO
    {
        public int GameId { get; set; }
        public string Player1Id { get; set; }
        public string Player2Id { get; set; }
    }
}
