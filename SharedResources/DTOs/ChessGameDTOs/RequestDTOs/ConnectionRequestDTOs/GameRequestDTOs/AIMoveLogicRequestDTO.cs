using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class AIMoveLogicRequestDTO : RequestDTO
    {
        public BoardStateRequestDTO BoardRequestDTO { get; set; }
    }
}
