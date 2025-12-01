namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class SubmitMoveResponseDTO
    {
        public bool IsKingChecked { get; set; }
        public bool IsKingMate { get; set; }
        public bool IsMoveSuccess { get; set; }
    }
}
