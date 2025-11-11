using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs
{
    public class UserConnectionResponseDTO : ICheseGameResponseDTO
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public Gameinfo Gameinfo { get; set; }
        public Guid GameId { get ; set ; }
    }
    public class Gameinfo
    {
        public KeyValuePair<string, string> Players { get; set; }
    }
}
