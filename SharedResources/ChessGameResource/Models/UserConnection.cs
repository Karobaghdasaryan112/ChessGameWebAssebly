namespace SharedResources.ChessGameResource.Models
{
    public class UserConnection
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public Gameinfo Gameinfo { get; set; }
    }
    public class Gameinfo
    {
        public Guid GameId { get; set; }
        public KeyValuePair<string, string> Players { get; set; }
        //public enum lvl
    }
}
