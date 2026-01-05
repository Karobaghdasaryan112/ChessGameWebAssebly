using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs
{
    public class AIMoveLogicRequestDTO : IRequestDTO
    {
        public BoardStateRequestDTO BoardRequestDTO { get; set; }
    }
}
