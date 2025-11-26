using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.Responses.ResponseMessages;
namespace ChessGame.Core.Services.Services.HubServices
{
    public class GameService<THub> : IGameService<THub> where THub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IConnectionService<THub> _connectionService;
        private readonly IBoardService _boardService;
        private BaseHubService<THub> _baseHubService;

        public GameService(IConnectionService<THub> connectionService, IBoardService boardService, BaseHubService<THub> baseHubService)
        {
            _boardService = boardService;
            _connectionService = connectionService;
            _baseHubService = baseHubService;
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

        public async Task<bool> SendIsSameFigureClickedAsync(Position selectedPosition, Position currentPosition, Guid gameId)
        {
            var gameState =
                ActiveGames.ActiveGamesAndBoards.
                Where(kvp =>
                kvp.Key == gameId).
                First().Value;

            var currentPositionBlock = gameState.GetBlockByPosition(currentPosition);
            var selectedPositionBlock = gameState.GetBlockByPosition(selectedPosition);
            return currentPositionBlock?.Figure?.FigureColor == selectedPositionBlock?.Figure?.FigureColor;
        }


        public async Task<ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(ConnectionRequestDTO<MoveRequestDTO> sendMoveConnectionRequestDTO)
        {
            var invalidResponse = ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                                 new MoveResponseDTO()
                                 {
                                     GameId = sendMoveConnectionRequestDTO.Data.GameId,
                                     Player = sendMoveConnectionRequestDTO.Data.Player
                                 },
                                 ChessGameResponseMessage.InvalidMove,
                                 System.Net.HttpStatusCode.BadRequest);


            //current Board State from Server
            var gameState = ActiveGames.ActiveGamesAndBoards[sendMoveConnectionRequestDTO.Data.GameId];

            var currentPositionBlock = gameState.GetBlockByPosition(sendMoveConnectionRequestDTO.Data.CurrentPosition);

            if (currentPositionBlock.EventColor != EventColors.Cut &&
                currentPositionBlock.EventColor != EventColors.Move)
                return invalidResponse;


            var boardStateRequestDTO =
                new BoardStateRequestDTO()
                {
                    GameId = sendMoveConnectionRequestDTO.Data.GameId,
                    CutableFigure = default,
                    Player = sendMoveConnectionRequestDTO.Data.Player,
                    From = sendMoveConnectionRequestDTO.Data.From,
                    To = sendMoveConnectionRequestDTO.Data.To,
                    OpponentColor =
                    sendMoveConnectionRequestDTO.Data.MyColor == FigureColors.Black ? FigureColors.White : FigureColors.Black,
                };


            if (currentPositionBlock.EventColor == SharedResources.ChessGameResource.Enums.Colors.EventColors.Move)
                return await MoveLogic(gameState, boardStateRequestDTO, sendMoveConnectionRequestDTO);


            if (currentPositionBlock.EventColor == SharedResources.ChessGameResource.Enums.Colors.EventColors.Cut)
                return await CutLogic(gameState, boardStateRequestDTO, sendMoveConnectionRequestDTO);

            return invalidResponse;
        }

        public async Task<ConnectionResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(ConnectionRequestDTO<ClickRequestDTO> sendClickConnectionRequestDTO)
        {
            var gameState = ActiveGames.ActiveGamesAndBoards[sendClickConnectionRequestDTO.Data.GameId];

            var currentPositionBlock = gameState.GetBlockByPosition(sendClickConnectionRequestDTO.Data.CurrentPosition);

            //if the request is movable or cutable then return the block from the Active Games (current Board State from Server)
            var canMoveResultBlock = await _boardService.CanClick(sendClickConnectionRequestDTO.Data.MyColor, currentPositionBlock, sendClickConnectionRequestDTO.Data.PreviusBlockInformationDTO, gameState);

            if (canMoveResultBlock == default)
                return ConnectionResponseDTO<ClickResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                   new ClickResponseDTO()
                   {
                       GameId = sendClickConnectionRequestDTO.Data.GameId,
                       Player = sendClickConnectionRequestDTO.Data.Player
                   },
                   ChessGameResponseMessage.InvalidMove,
                   System.Net.HttpStatusCode.BadRequest);

            ResetEventableBlocks(gameState);

            var positions =
            gameState.
            GetBlockByPosition(sendClickConnectionRequestDTO.Data.From).
            Figure.
            GetMovableAndCutableBlocks(sendClickConnectionRequestDTO.Data.From, gameState);

            return ConnectionResponseDTO<ClickResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new ClickResponseDTO()
                {
                    CutableBlocks = positions.CutableBlock,
                    MovableBlocks = positions.MovableBlock,
                    GameId = sendClickConnectionRequestDTO.Data.GameId,
                    Player = sendClickConnectionRequestDTO.Data.Player
                },
                ChessGameResponseMessage.SuccessUserConnections,
                System.Net.HttpStatusCode.OK);
        }





        //Privet Methods
        private void ResetEventableBlocks(Board gameState)
        {
            //reset the previus selected Blocks(Movable and cutable)
            var eventableBoardBlocks = gameState.BoardBlocks!.SelectMany(blockI => blockI.Where(blockJ => blockJ.EventColor == EventColors.Cut || blockJ.EventColor == EventColors.Move).ToArray());

            foreach (var eventableBoardBlock in eventableBoardBlocks)
                eventableBoardBlock.EventColor = EventColors.None;
        }



        private async Task<ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> MoveLogic(Board gameState, BoardStateRequestDTO boardStateRequestDTO, ConnectionRequestDTO<MoveRequestDTO> sendMoveConnectionRequestDTO)
        {
            var moveResult = await _boardService.SubmitMoveAsync(sendMoveConnectionRequestDTO.Data.GameId, sendMoveConnectionRequestDTO.Data.From, sendMoveConnectionRequestDTO.Data.To, gameState);

            ResetEventableBlocks(gameState);

            if (!moveResult)
                return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                new MoveResponseDTO()
                {
                    GameId = sendMoveConnectionRequestDTO.Data.GameId,
                    Player = sendMoveConnectionRequestDTO.Data.Player
                },
                ChessGameResponseMessage.InvalidMove,
                System.Net.HttpStatusCode.BadRequest);

            await _connectionService.SendBoardStateToOpponentClient(new ConnectionRequestDTO<BoardStateRequestDTO>() { Data = boardStateRequestDTO });

            gameState.SwitchTurn();

            return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new MoveResponseDTO()
                {
                    GameId = sendMoveConnectionRequestDTO.Data.GameId,
                    Player = sendMoveConnectionRequestDTO.Data.Player
                },
                ChessGameResponseMessage.MoveSuccessful,
                System.Net.HttpStatusCode.OK);
        }

        //TO DO Create CutLogic
        private async Task<ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> CutLogic(Board gameState, BoardStateRequestDTO boardStateRequestDTO, ConnectionRequestDTO<MoveRequestDTO> sendMoveConnectionRequestDTO)
        {
            throw new NotImplementedException();
        }
    }
}
