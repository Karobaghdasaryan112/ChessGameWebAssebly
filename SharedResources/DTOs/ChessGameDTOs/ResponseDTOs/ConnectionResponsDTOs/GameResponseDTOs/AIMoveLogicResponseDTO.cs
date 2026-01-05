using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class AIMoveLogicResponseDTO : IResponseDTO
    {
       public MoveResponseDTO? MoveResponseDTO { get; set; }
    }
}
