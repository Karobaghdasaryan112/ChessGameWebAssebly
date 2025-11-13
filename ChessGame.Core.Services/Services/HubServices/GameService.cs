using ChessGame.Core.Services.Contracts.Hub;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class GameService<T> : IGameService<T> where T : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IConnectionService<T> _connectionService;
        public GameService(IConnectionService<T> connectionService)
        {
            _connectionService = connectionService;
        }

        public Task ClearGameAsync(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(ConnectionRequestDTO<GetONlinePlayersRequestDTO> getOnlinePlayersRequestDTO)
        {
            var onlinePlayers = _connectionService.
                CurrentConnectionState
                .Where(connectionKeyValuePair => connectionKeyValuePair.Key != getOnlinePlayersRequestDTO.Data.UserGuid)
                .ToDictionary();
            if (onlinePlayers.Count == 0)
                return
                ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>
                .CreateErrorResponse(
                    null,
                    ChessGameResponseMessage.UserConnectionNotFound,
                    System.Net.HttpStatusCode.OK);
            return
                ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(
                    new GetOnlinePlayersResponseDTO()
                    {
                        OnlinePlayers = onlinePlayers
                    },
                    ChessGameResponseMessage.SuccessUserConnections,
                    System.Net.HttpStatusCode.OK);
        }


        public Task<IResponseTypes<UserConnectionDTO, ChessGameResponseMessage>> SendGameStateAsync(Guid gameId)
        {
            throw new NotImplementedException();
        }

    }
}
