using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class IsKingMateResponseDTO : IResponseDTO
    {
        public bool IsKingMate { get; set; }
    }
}
