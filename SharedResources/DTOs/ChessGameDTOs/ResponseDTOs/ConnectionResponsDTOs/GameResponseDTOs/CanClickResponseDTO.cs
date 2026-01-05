using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class CanClickResponseDTO : IResponseDTO
    {
        public Block ClickedBlock { get; set; }
        public List<CastlingInfosDTO>? CastlingInfosDTO { get; set; }
    }
}
