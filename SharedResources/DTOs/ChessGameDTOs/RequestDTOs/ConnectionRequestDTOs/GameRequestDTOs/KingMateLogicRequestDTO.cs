using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class KingMateLogicRequestDTO : IRequestDTO
    {
        public BoardStateRequestDTO boardStateRequestDTO { get; set; }
        public bool IsTrainingGame = false;
        public bool isComputerWin = false;
    }
}
