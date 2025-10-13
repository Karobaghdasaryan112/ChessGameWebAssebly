using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs
{
    public class MoveDTOs : ICheseGameRequestDTO, ICheseGameResponseDTO
    {
        public int GameId { get; set; }
    }
}
