using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class IsKingCheckedResponseDTO : IResponseDTO
    {
        public bool IsKingChecked { get; set; }
    }
}
