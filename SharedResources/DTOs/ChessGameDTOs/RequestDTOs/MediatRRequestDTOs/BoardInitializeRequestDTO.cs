using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs
{
    public class BoardInitializeRequestDTO : ICheseGameRequestDTO
    {
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public TimeSpan Player1Time { get; set; }
        public TimeSpan Player2Time { get; set; }
        public GameEvent GameEvent { get; set; }
        public Guid Player1Id { get; set; }
        public Guid Player2Id { get; set; }
        public Guid GameId { get ; set; }
    }
}
