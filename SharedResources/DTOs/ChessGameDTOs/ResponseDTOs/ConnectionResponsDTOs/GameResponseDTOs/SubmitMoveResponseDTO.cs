using SharedResources.ChessGameResource.Models;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class SubmitMoveResponseDTO
    {
        public bool IsKingChecked { get; set; }
        public bool IsKingMate { get; set; }
        public bool IsMoveSuccess { get; set; }
        public CastlingRookPositions CastlingRookPositions { get; set; }
    }
    public class CastlingRookPositions
    {
        public Position RookFrom { get; set; }
        public Position RookTo { get; set; }
    }
}
