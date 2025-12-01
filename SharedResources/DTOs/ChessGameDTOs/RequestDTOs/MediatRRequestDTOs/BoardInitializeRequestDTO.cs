using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs
{
    public class BoardInitializeRequestDTO : ICheseGameRequestDTO
    {
        public Guid Player1Id { get; set; }
        public Guid Player2Id { get; set; }
        public Guid GameId { get ; set; }
    }
}
