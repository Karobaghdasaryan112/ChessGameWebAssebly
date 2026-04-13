using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class SaveGameEventAndWinnerRequestDTO : RequestDTO
    {
        public Guid GameId { get; set; }
        public Guid WinnerPlayerGuid { get; set; }
    }
}
