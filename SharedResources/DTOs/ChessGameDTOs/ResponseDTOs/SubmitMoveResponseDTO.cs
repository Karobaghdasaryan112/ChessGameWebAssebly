using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs
{
    public class SubmitMoveResponseDTO : ICheseGameResponseDTO
    {
        public string Player { get ; set ; }
        public bool CanMove { get; set; }
        public Guid GameId { get; set; }
    }
}
