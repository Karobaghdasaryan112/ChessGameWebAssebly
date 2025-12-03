namespace SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs
{
    public class UserConnectionDTO
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public Gameinfo? Gameinfo { get; set; }
        public Guid GameId { get; set; }
    }
    public class Gameinfo
    {
        public KeyValuePair<Guid, Guid> Players { get; set; }
    }
}
