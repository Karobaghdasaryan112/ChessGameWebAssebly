using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class SubmitMoveResponseDTO : IResponseDTO
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
