using ChessGame.Core.Services.Contracts.BoardServices;
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
        private readonly IBoardService _boardService;

        public GameService(IConnectionService<T> connectionService, IBoardService boardService)
        {
            _boardService = boardService;
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
        public async Task<ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(ConnectionRequestDTO<MoveRequestDTO> sendMoveConnectionRequestDTO)
        {
            var canMoveResult = await _boardService.CanClick(sendMoveConnectionRequestDTO.Data.MyColor, sendMoveConnectionRequestDTO.Data.CurrentBlock, sendMoveConnectionRequestDTO.Data.PreviusBlockInformationDTO);
            
            if (!canMoveResult)
                return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                   new MoveResponseDTO()
                   {
                       GameId = sendMoveConnectionRequestDTO.Data.GameId,
                       Player = sendMoveConnectionRequestDTO.Data.Player
                   },
                   ChessGameResponseMessage.InvalidMove,
                   System.Net.HttpStatusCode.BadRequest);

            var gameState =
                ActiveGames.ActiveGamesAndBoards.
                Where(kvp =>
                kvp.Key == sendMoveConnectionRequestDTO.Data.GameId).
                First().Value;

            gameState.FigureColor = sendMoveConnectionRequestDTO.Data.MyColor;

            if (sendMoveConnectionRequestDTO.Data.To == null)
            {
                var positions =
                gameState.
                GetBlockByPosition(sendMoveConnectionRequestDTO.Data.From).
                Figure.
                GetMovableAndCutableBlocks(sendMoveConnectionRequestDTO.Data.From, gameState);

                return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    new MoveResponseDTO()
                    {
                        CutableBlocks = positions.CutablePositions,MovableBlocks = positions.MovablePositions,
                        GameId = sendMoveConnectionRequestDTO.Data.GameId,Player = sendMoveConnectionRequestDTO.Data.Player
                    },
                    ChessGameResponseMessage.SuccessUserConnections,
                    System.Net.HttpStatusCode.OK);
            }
            if(sendMoveConnectionRequestDTO.Data.CurrentBlock.EventColor != SharedResources.ChessGameResource.Enums.Colors.EventColors.Cut)
                return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                                 new MoveResponseDTO()
                                 {
                                     GameId = sendMoveConnectionRequestDTO.Data.GameId,
                                     Player = sendMoveConnectionRequestDTO.Data.Player
                                 },
                                 ChessGameResponseMessage.InvalidMove,
                                 System.Net.HttpStatusCode.BadRequest);

            var moveResult = await _boardService.SubmitMoveAsync(sendMoveConnectionRequestDTO.Data.GameId, sendMoveConnectionRequestDTO.Data.From, sendMoveConnectionRequestDTO.Data.To, gameState);
            if (moveResult)
                return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    new MoveResponseDTO()
                    {
                        GameId = sendMoveConnectionRequestDTO.Data.GameId,Player = sendMoveConnectionRequestDTO.Data.Player
                    },
                    ChessGameResponseMessage.MoveSuccessful,
                    System.Net.HttpStatusCode.OK);

            return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                new MoveResponseDTO()
                {
                    GameId = sendMoveConnectionRequestDTO.Data.GameId,Player = sendMoveConnectionRequestDTO.Data.Player
                },
                ChessGameResponseMessage.InvalidMove,
                System.Net.HttpStatusCode.BadRequest);
        }
    }
}
