using ChessGame.Core.Services.Contracts.Hub;
using SharedResources.ChessGameResource.StaticResources;
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

        public async Task<ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(ConnectionRequestDTO<GetONlinePlayersRequestDTO> connectionRequestDTO)
        {
            var onlinePlayers = _connectionService.
                CurrentConnectionState
                .Where(connectionKeyValuePair => connectionKeyValuePair.Key != connectionRequestDTO.Data.UserGuid)
                .ToDictionary();
            if (onlinePlayers.Count() == 0)
                return
                    ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>
                .CreateErrorResponse(
                    default,
                    ChessGameResponseMessage.UserConnectionNotFound,
                    System.Net.HttpStatusCode.BadRequest);
            return
                ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(
                    new GetOnlinePlayersResponseDTO() { OnlinePlayers = onlinePlayers },
                    ChessGameResponseMessage.UserConnectionFoundSuccess,
                    System.Net.HttpStatusCode.OK);
        }


        public async Task<ConnectionResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(ConnectionRequestDTO<SendGameStateReqeustDTO> gameStateReqeustDTO)
        {
            var games = ActiveGames.ActiveGamesAndBoards;
            var gameState = ActiveGames.ActiveGamesAndBoards.Where(kvp => kvp.Key == gameStateReqeustDTO.Data.GameId).FirstOrDefault();
            return await Task.Run(() => new ConnectionResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>()
            {
                Data = new SendGameStateResponseDTO()
                {
                    Board = gameState.Value
                },
                Message = ChessGameResponseMessage.GameCreated,
            });
        }
    }
}
