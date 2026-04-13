using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class GetAllHistoryWidgetRequestDTO : RequestDTO
    {
        public Guid CurrentPlayerId { get; set; }
    }
}
