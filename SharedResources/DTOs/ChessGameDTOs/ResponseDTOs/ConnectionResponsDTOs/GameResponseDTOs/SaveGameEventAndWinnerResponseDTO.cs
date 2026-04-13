using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class SaveGameEventAndWinnerResponseDTO : IResponseDTO
    {
        public bool IsSaved { get; set; }

    }
}
