using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class KingMateLogicRequestDTO : RequestDTO
    {
        public BoardStateRequestDTO boardStateRequestDTO { get; set; }
        public bool IsTrainingGame = false;
        public bool isComputerWin = false;
    }
}
