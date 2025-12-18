namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class SaveGameEventAndWinnerRequestDTO
    {
        public Guid GameId { get; set; }
        public Guid WinnerPlayerGuid { get; set; }
    }
}
