using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Extentions;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Core.Services.Services.Validations;
using MediatR;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.ChessGameResource.Models;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Requests;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class GameService(
        IMediator mediator,
        IConnectionService connectionService,
        IBoardService boardService,
        BaseHubService baseHubService,
        GenericValidationService validationService)
        : IGameService
    {
        private BaseHubService _baseHubService = baseHubService;

        public Task ClearGameAsync(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>>
            GetOnlinePlayersAsync(ConnectionRequestDTO<GetONlinePlayersRequestDTO> connectionRequestDTO)
        {
            //Validate the Request Data
            var validationResult = (await validationService.ValidateAsync(connectionRequestDTO.Data));
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(GetOnlinePlayersResponseDTO)))!;


            var onlinePlayers = connectionService.CurrentConnectionState
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


        public async Task<ConnectionResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(
            ConnectionRequestDTO<SendGameStateReqeustDTO> gameStateReqeustDTO)
        {
            //Validate the Request Data
            var validationResult = (await validationService.ValidateAsync(gameStateReqeustDTO.Data));
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(SendGameStateResponseDTO)))!;


            ActiveGames.ActiveGamesAndBoards.TryGetValue(gameStateReqeustDTO.Data.GameId, out var gameState);

            return await Task.FromResult(new ConnectionResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>()
            {
                Data = new SendGameStateResponseDTO()
                {
                    Board = gameState
                },
                Message = ChessGameResponseMessage.GameCreated,
            });
        }


        public async Task<bool> SendIsSameFigureClickedAsync(Position selectedPosition, Position currentPosition,
            Guid gameId)
        {
            var gameState =
                ActiveGames.ActiveGamesAndBoards[gameId];

            var currentPositionBlock = gameState?.GetBlockByPosition(currentPosition);
            var selectedPositionBlock = gameState?.GetBlockByPosition(selectedPosition);
            return await Task.FromResult(currentPositionBlock?.Figure?.FigureColor ==
                                         selectedPositionBlock?.Figure?.FigureColor);
        }



        public async Task<ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(
            ConnectionRequestDTO<MoveRequestDTO> sendMoveConnectionRequestDTO)
        {
            //Validate the Request Data
            var validationResult = (await validationService.ValidateAsync(sendMoveConnectionRequestDTO.Data));
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

            if (currentPositionBlock.EventColor != EventColors.Cut && currentPositionBlock.EventColor != EventColors.Move)
                return invalidResponse;


            var boardStateRequestDTO =
                new BoardStateRequestDTO
                {
                    GameId = sendMoveConnectionRequestDTO.Data.GameId,
                    CutableFigure = default,
                    Player = sendMoveConnectionRequestDTO.Data.Player,
                    From = sendMoveConnectionRequestDTO.Data.From,
                    To = sendMoveConnectionRequestDTO.Data.To,
                    GameState = gameState,
                    OpponentColor =
                        sendMoveConnectionRequestDTO.Data.MyColor == FigureColors.Black
                            ? FigureColors.White
                            : FigureColors.Black,
                    IsReadyToEvent =
                        currentPositionBlock.EventColor == EventColors.Move
                            ? IsReady.IsReadyToMove :
                        currentPositionBlock.EventColor == EventColors.Cut
                            ? IsReady.IsReadyToCut :
                        IsReady.None
                };

            var moveLogicCommandHandler =
                new MoveLogicCommand<IRequestTypes<BoardStateRequestDTO>,
                    IResponseTypes<MoveResponseDTO, ChessGameResponseMessage>>(
                    new ChessGameRequest<BoardStateRequestDTO>()
                    {
                        requestType = boardStateRequestDTO,
                    });

            if (boardStateRequestDTO.IsReadyToEvent == IsReady.None)
                return invalidResponse;
            else
            {
                var moveCommandResponse = await mediator.Send(moveLogicCommandHandler);
                return moveCommandResponse.IsSuccess
                    ? ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        moveCommandResponse.Data,
                        moveCommandResponse.message,
                        moveCommandResponse.StatusCode)
                    : ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        moveCommandResponse.Data,
                        moveCommandResponse.message,
                        moveCommandResponse.StatusCode);
            }
        }

        public async Task<ConnectionResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(
            ConnectionRequestDTO<ClickRequestDTO> sendClickConnectionRequestDTO)
        {
            //Validate the Request Data
            var validationResult = (await validationService.ValidateAsync(sendClickConnectionRequestDTO.Data));
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(ClickResponseDTO)))!;


            var gameState = ActiveGames.ActiveGamesAndBoards[sendClickConnectionRequestDTO.Data.GameId];

            var currentPositionBlock = gameState.GetBlockByPosition(sendClickConnectionRequestDTO.Data.CurrentPosition);



            var requestDTO = new ChessGameRequest<CanClickRequestDTO>()
            {
                requestType = new()
                {
                    ClickedBlockInformationDto = sendClickConnectionRequestDTO.Data.PreviusBlockInformationDTO,
                    CurrentBlock = currentPositionBlock,
                    CurrentBoardBoardState = gameState,
                    FigureColor = sendClickConnectionRequestDTO.Data.MyColor
                }
            };
            var sendClickQuery = new SendClickQuery<
                IRequestTypes<CanClickRequestDTO>,
                IResponseTypes<CanClickResponseDTO, ChessGameResponseMessage>>(requestDTO);



            var canClickResponse = await mediator.Send<IResponseTypes<CanClickResponseDTO, ChessGameResponseMessage>>(sendClickQuery);

            if (!canClickResponse.IsSuccess)
                return ConnectionResponseDTO<ClickResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new ClickResponseDTO()
                    {
                        GameId = sendClickConnectionRequestDTO.Data.GameId,
                        Player = sendClickConnectionRequestDTO.Data.Player
                    },
                    ChessGameResponseMessage.InvalidMove,
                    System.Net.HttpStatusCode.BadRequest);

            gameState.ResetEventableBlocks();

            var positions =
                gameState.GetBlockByPosition(sendClickConnectionRequestDTO.Data.From).Figure
                    .GetMovableAndCuttableBlocks(sendClickConnectionRequestDTO.Data.From, gameState);

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
    }
}
