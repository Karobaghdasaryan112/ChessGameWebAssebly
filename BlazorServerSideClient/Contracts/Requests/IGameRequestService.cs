using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Requests
{
    public interface IGameRequestService
    {
        Task<ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(ConnectionRequestDTO<GetONlinePlayersRequestDTO> getOnlinePlayersRequestDTO);
        Task<ConnectionResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(ConnectionRequestDTO<SendGameStateReqeustDTO> gameStateReqeustDTO);
        Task ClearGameAsync(Guid gameId);
    }
}
