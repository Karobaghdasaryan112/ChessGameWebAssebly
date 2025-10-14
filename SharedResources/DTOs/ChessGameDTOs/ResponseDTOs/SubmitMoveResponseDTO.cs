using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs
{
    public class SubmitMoveResponseDTO : ICheseGameResponseDTO
    {
        public int GameId { get; set; }
        public string Player { get ; set ; }
        public bool CanMove { get; set; }
    }
}
