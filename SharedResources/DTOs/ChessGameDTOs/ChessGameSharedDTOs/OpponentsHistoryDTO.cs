namespace SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs
{
    public class OpponentsHistoryDTO
    {
        public string Opponent { get; set; }
        public Guid OpponentGuid { get; set; }
        public int TotalCount { get; set; }
        public Guid GameId { get; set; }
    }
}
