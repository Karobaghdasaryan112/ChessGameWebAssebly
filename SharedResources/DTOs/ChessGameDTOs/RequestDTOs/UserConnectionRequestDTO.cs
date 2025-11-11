using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs
{
    public class UserConnectionRequestDTO : ICheseGameRequestDTO
    {
        public Guid GameId { get ; set ; }
    }
}
