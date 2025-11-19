using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IGameService<T> where T : Microsoft.AspNetCore.SignalR.Hub
    {
        Task<ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(ConnectionRequestDTO<GetONlinePlayersRequestDTO> connectionRequestDTO);
        Task<ConnectionResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(ConnectionRequestDTO<SendGameStateReqeustDTO> gameStateReqeustDTO);
        Task ClearGameAsync(Guid gameId);
    }
}
