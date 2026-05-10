using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class RemoveUserFromGameResponseDTO : IResponseDTO
    {
        public bool IsRemoved { get; set; }
    }
}
