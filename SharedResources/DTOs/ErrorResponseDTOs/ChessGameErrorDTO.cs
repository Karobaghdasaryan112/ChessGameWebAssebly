using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ErrorResponseDTOs
{
    public class ChessGameErrorDTO : ICheseGameResponseDTO
    {
        public Guid GameId { get; set; }
    }
}
