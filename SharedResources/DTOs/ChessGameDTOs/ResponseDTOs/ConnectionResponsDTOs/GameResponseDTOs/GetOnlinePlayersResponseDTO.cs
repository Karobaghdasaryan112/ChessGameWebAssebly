using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs
{
    public class GetOnlinePlayersResponseDTO
    {
        public Dictionary<Guid, UserConnectionDTO> OnlinePlayers { get; set; } 
    }
}
