using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class CanClickResponseDTO : IResponseDTO
    {
        public Block ClickedBlock { get; set; }
    }
}
