using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs
{
    public class MoveDTOs : ICheseGameRequestDTO, ICheseGameResponseDTO
    {
        public Guid GameId { get ; set ; }
    }
}
