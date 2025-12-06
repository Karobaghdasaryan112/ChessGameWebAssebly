using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs
{
    public class SendGameStateReqeustDTO : IRequestDTO
    {
        public Guid GameId {  get; set; }
    }
}
