using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class GetOnlinePlayersResponseDTO : IResponseDTO
    {
        public Dictionary<Guid, UserConnectionDTO> OnlinePlayers { get; set; } 
    }
}
