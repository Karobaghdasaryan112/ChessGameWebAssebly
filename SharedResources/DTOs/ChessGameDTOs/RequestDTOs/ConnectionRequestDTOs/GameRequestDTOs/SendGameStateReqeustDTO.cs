using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs
{
    public class SendGameStateReqeustDTO : RequestDTO
    {
        public Guid GameId {  get; set; }
    }
}
