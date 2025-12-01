using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Services.Validations;
using FluentValidation;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using SubmitMoveRequestDTO = SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs.SubmitMoveRequestDTO;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class GameService<THub>(
        IConnectionService<THub> connectionService,
        IBoardService boardService,
        BaseHubService<THub> baseHubService,
        GameValidationService gameValidationService)
        : IGameService<THub>
        where THub : Microsoft.AspNetCore.SignalR.Hub
    {
        private BaseHubService<THub> _baseHubService = baseHubService;

        public Task ClearGameAsync(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(ConnectionRequestDTO<GetONlinePlayersRequestDTO> connectionRequestDTO)
        {
            //Validate the Request Data
            var validationResult = (await gameValidationService.ValidateAsync(connectionRequestDTO.Data));
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(GetOnlinePlayersResponseDTO)))!;



            var onlinePlayers = connectionService.
                CurrentConnectionState
                .Where(connectionKeyValuePair => connectionKeyValuePair.Key != connectionRequestDTO.Data.UserGuid)
                .ToDictionary();
            if (!onlinePlayers.Any())
                return
                    ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>
                .CreateErrorResponse(
                    null!,
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
            //Validate the Request Data
            var validationResult = (await gameValidationService.ValidateAsync(gameStateReqeustDTO.Data));
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(SendGameStateResponseDTO)))!;



            var games = ActiveGames.ActiveGamesAndBoards;
            var gameState = ActiveGames.ActiveGamesAndBoards[gameStateReqeustDTO.Data.GameId];

            return await Task.FromResult(new ConnectionResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>()
            {
                Data = new SendGameStateResponseDTO()
                {
                    Board = gameState
                },
                Message = ChessGameResponseMessage.GameCreated,
            });
        }

        public async Task<bool> SendIsSameFigureClickedAsync(Position selectedPosition, Position currentPosition, Guid gameId)
        {
            var gameState =
                ActiveGames.ActiveGamesAndBoards[gameId];

            var currentPositionBlock = gameState?.GetBlockByPosition(currentPosition);
            var selectedPositionBlock = gameState?.GetBlockByPosition(selectedPosition);
            return await Task.FromResult(currentPositionBlock?.Figure?.FigureColor == selectedPositionBlock?.Figure?.FigureColor);
        }


        public async Task<ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(
            ConnectionRequestDTO<MoveRequestDTO> sendMoveConnectionRequestDTO)
        {
            //Validate the Request Data
            var validationResult = (await gameValidationService.ValidateAsync(sendMoveConnectionRequestDTO.Data));
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(MoveResponseDTO)))!;




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
                new BoardStateRequestDTO
                {
                    GameId = sendMoveConnectionRequestDTO.Data.GameId,
                    CutableFigure = default,
                    Player = sendMoveConnectionRequestDTO.Data.Player,
                    From = sendMoveConnectionRequestDTO.Data.From,
                    To = sendMoveConnectionRequestDTO.Data.To,
                    OpponentColor =
                    sendMoveConnectionRequestDTO.Data.MyColor == FigureColors.Black ? FigureColors.White : FigureColors.Black,
                    IsReadyToEvent = currentPositionBlock.EventColor == EventColors.Move ?
                        IsReady.IsReadyToMove :
                        currentPositionBlock.EventColor == EventColors.Cut ?
                            IsReady.IsReadyToCut :
                            IsReady.None
                };


            return boardStateRequestDTO.IsReadyToEvent == IsReady.None
                ? invalidResponse
                : await MoveLogic(gameState, boardStateRequestDTO, sendMoveConnectionRequestDTO);
        }

        public async Task<ConnectionResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(ConnectionRequestDTO<ClickRequestDTO> sendClickConnectionRequestDTO)
        {
            //Validate the Request Data
            var validationResult = (await gameValidationService.ValidateAsync(sendClickConnectionRequestDTO.Data));
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(ClickResponseDTO)))!;




            var gameState = ActiveGames.ActiveGamesAndBoards[sendClickConnectionRequestDTO.Data.GameId];

            var currentPositionBlock = gameState.GetBlockByPosition(sendClickConnectionRequestDTO.Data.CurrentPosition);

            //if the request is movable or cuttable then return the block from the Active Games (current Board State from Server)
            //CanClick RequestData
            var canClickRequest = new ConnectionRequestDTO<CanClickRequestDTO>()
            {
                Data = new CanClickRequestDTO()
                {
                    ClickedBlockInformationDto = sendClickConnectionRequestDTO.Data.PreviusBlockInformationDTO,
                    CurrentBlock = currentPositionBlock,
                    CurrentBoardBoardState = gameState,
                    FigureColor = sendClickConnectionRequestDTO.Data.MyColor
                }
            };
            var canMoveResultBlock = await boardService.CanClick(canClickRequest);

            if (!canMoveResultBlock.IsSuccess)
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
            //reset the previous selected Blocks(Movable and cuttable)
            var eventableBoardBlocks = gameState.BoardBlocks!.SelectMany(blockI => blockI.Where(blockJ => blockJ.EventColor == EventColors.Cut || blockJ.EventColor == EventColors.Move).ToArray());

            foreach (var eventableBoardBlock in eventableBoardBlocks)
                eventableBoardBlock.EventColor = EventColors.None;
        }



        private async Task<ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> MoveLogic(Board gameState, BoardStateRequestDTO boardStateRequestDTO, ConnectionRequestDTO<MoveRequestDTO> sendMoveConnectionRequestDTO)
        {
            var submitMoveRequest = new SubmitMoveRequestDTO()
            {
                From = sendMoveConnectionRequestDTO.Data.From,
                To = sendMoveConnectionRequestDTO.Data.To,
                CurrentBoardState = gameState,
                GameId = sendMoveConnectionRequestDTO.Data.GameId

            };
            var submitMoveConnectionResult = await boardService.SubmitMoveAsync(submitMoveRequest);

            ResetEventableBlocks(gameState);

            if (!submitMoveConnectionResult.IsSuccess)
                return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new MoveResponseDTO()
                    {
                        GameId = sendMoveConnectionRequestDTO.Data.GameId,
                        Player = sendMoveConnectionRequestDTO.Data.Player
                    },
                    ChessGameResponseMessage.InvalidMove,
                    System.Net.HttpStatusCode.BadRequest);

            if (submitMoveConnectionResult.Data.IsKingChecked)
            {
                var checkedKingForMe = gameState.GetBlockByFigureTypeAndColor(FigureType.King, (FigureColors)gameState.Turn);
                boardStateRequestDTO.IsKingChecked = true;
                boardStateRequestDTO.CheckedKingPosition = checkedKingForMe.Position;
                boardStateRequestDTO.From = default(Position);
                boardStateRequestDTO.To = default(Position);
                await connectionService.SendBoardStateToClient(new ConnectionRequestDTO<BoardStateRequestDTO>() { Data = boardStateRequestDTO }, boardStateRequestDTO.Player, true);

                return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new MoveResponseDTO()
                    {
                        GameId = sendMoveConnectionRequestDTO.Data.GameId,
                        Player = sendMoveConnectionRequestDTO.Data.Player
                    },
                    ChessGameResponseMessage.InvalidMove,
                    System.Net.HttpStatusCode.BadRequest);
            }

            gameState.SwitchTurn();

            await connectionService.SendBoardStateToClient(new ConnectionRequestDTO<BoardStateRequestDTO>() { Data = boardStateRequestDTO }, boardStateRequestDTO.Player, true);

            var isKingChecked = await boardService.IsKingCheckedAsync(gameState, gameState.Turn);
            var checkedKingForOpponent = gameState.GetBlockByFigureTypeAndColor(FigureType.King, (FigureColors)gameState.Turn);
            boardStateRequestDTO.IsKingChecked = isKingChecked;
            boardStateRequestDTO.CheckedKingPosition = checkedKingForOpponent.Position;

            await connectionService.SendBoardStateToClient(new ConnectionRequestDTO<BoardStateRequestDTO>() { Data = boardStateRequestDTO }, boardStateRequestDTO.Player, false);


            return ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new MoveResponseDTO()
                {
                    GameId = sendMoveConnectionRequestDTO.Data.GameId,
                    Player = sendMoveConnectionRequestDTO.Data.Player,
                    IsReadyToEvent = IsReady.IsReadyToCut
                },
                ChessGameResponseMessage.MoveSuccessful,
                System.Net.HttpStatusCode.OK);
        }
    }
}
