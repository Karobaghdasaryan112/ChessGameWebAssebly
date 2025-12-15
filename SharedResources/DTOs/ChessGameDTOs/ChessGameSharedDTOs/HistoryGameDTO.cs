using SharedResources.ChessGameResource.Enums.Events;

namespace SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs
{
    public class HistoryGameDTO
    {
        public string Opponent { get; set; }
        public Guid OpponentGuid { get; set; }
        public GameEvent GameEvent { get; set; }

        public TimeSpan Duration { get; set; }
        public DateTime Date { get; set; }
    }
}
