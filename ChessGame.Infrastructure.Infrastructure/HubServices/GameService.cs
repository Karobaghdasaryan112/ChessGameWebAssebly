using ChessGame.Core.Services.Constants;
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
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
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

        public async Task<ResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>>
            GetOnlinePlayersAsync(GetONlinePlayersRequestDTO connectionRequestDTO)
        {
            var validationResult = (await validationService.ValidateAsync(connectionRequestDTO));
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(GetOnlinePlayersResponseDTO)))!;


            var onlinePlayers = connectionService.CurrentConnectionState
                .Where(connectionKeyValuePair => connectionKeyValuePair.Key != connectionRequestDTO.UserGuid)
                .ToDictionary();
            if (!onlinePlayers.Any())
                return
                    ResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>
                        .CreateErrorResponse(
                            null!,
                            ChessGameResponseMessage.UserConnectionNotFound,
                            System.Net.HttpStatusCode.BadRequest);
            return
                ResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>
                    .CreateSuccessResponse(
                        new GetOnlinePlayersResponseDTO() { OnlinePlayers = onlinePlayers },
                        ChessGameResponseMessage.UserConnectionFoundSuccess,
                        System.Net.HttpStatusCode.OK);
        }
        public async Task<ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>> RequestTrainingGameAsync(
            TrainingGameRequestDTO trainingGameRequestDTO)
        {
            var validationResult = (await validationService.ValidateAsync(trainingGameRequestDTO));
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(TrainingGameResponseDTO)))!;
            var GameId = Guid.NewGuid();

            HelperConstants.MAX_DEPTH = (int)trainingGameRequestDTO.TrainingDifficulty;

            var playerGuid = trainingGameRequestDTO.Player1Guid == Guid.Empty ?
                trainingGameRequestDTO.Player2Guid : trainingGameRequestDTO.Player1Guid;

            var playerName = trainingGameRequestDTO.Player1Guid == Guid.Empty ?
                trainingGameRequestDTO.Player2Name : trainingGameRequestDTO.Player1Name;
            var boardInitializeRequest = new SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs.BoardInitializeRequestDTO
            {
                GameEvent = GameEvent.Training,
                Player1Name = trainingGameRequestDTO.Player1Name,
                Player2Name = trainingGameRequestDTO.Player2Name,
                Player1Id = trainingGameRequestDTO.Player1Guid,
                Player2Id = trainingGameRequestDTO.Player2Guid,
                Player1Time = TimeSpan.FromMinutes((int)PlayEvent.Classical),
                Player2Time = TimeSpan.FromMinutes((int)PlayEvent.Classical),
            };

            var gameState =
                await boardService.InitializeBoardAsync(boardInitializeRequest);



            if (!gameState.IsSuccess)
            {
                return ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new TrainingGameResponseDTO()
                    {
                        GameId = gameState.Data.GameId,
                    },
                    ChessGameResponseMessage.GameCreationFailed,
                    System.Net.HttpStatusCode.InternalServerError);
            }

            gameState.Data.board = new Board(default(FigureColors));

            connectionService.CurrentConnectionState.TryAdd(
                playerGuid, new UserConnectionDTO()
                {
                    ConnectionId = trainingGameRequestDTO.ClientConnectionId,
                    GameId = gameState.Data.GameId,
                    UserName = playerName,
                    Gameinfo =
                    new Gameinfo()
                    {
                        Players = new KeyValuePair<Guid, Guid>(trainingGameRequestDTO.Player1Guid, trainingGameRequestDTO.Player2Guid)
                    }
                });

            ActiveGames.ActiveGamesAndBoards.TryAdd(gameState.Data.GameId, gameState.Data.board);
            var boardStateRespionseDTO =
                 new BoardStateRequestDTO
                 {
                     GameId = gameState.Data.GameId,
                     GameState = gameState.Data.board,

                 };
            await connectionService.SendBoardStateToClient(boardStateRespionseDTO, trainingGameRequestDTO.Player1Guid == Guid.Empty ? trainingGameRequestDTO.Player2Name : trainingGameRequestDTO.Player1Name, true);

            return await Task.FromResult(ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new TrainingGameResponseDTO()
                {
                    GameId = gameState.Data.GameId,
                    Board = gameState.Data.board
                },
                ChessGameResponseMessage.GameCreated,
                System.Net.HttpStatusCode.OK));

        }


        public async Task<ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(
            SendGameStateReqeustDTO gameStateReqeustDTO)
        {
            var validationResult = (await validationService.ValidateAsync(gameStateReqeustDTO));
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(SendGameStateResponseDTO)))!;


            ActiveGames.ActiveGamesAndBoards.TryGetValue(gameStateReqeustDTO.GameId, out var gameState);

            return await Task.FromResult(new ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>()
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

        public async Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(
            MoveRequestDTO sendMoveConnectionRequestDTO)
        {
            var validationResult = (await validationService.ValidateAsync(sendMoveConnectionRequestDTO));
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(MoveResponseDTO)))!;


            var invalidResponse = ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                new MoveResponseDTO()
                {
                    GameId = sendMoveConnectionRequestDTO.GameId,
                    Player = sendMoveConnectionRequestDTO.Player
                },
                ChessGameResponseMessage.InvalidMove,
                System.Net.HttpStatusCode.BadRequest);


            var gameState = ActiveGames.ActiveGamesAndBoards[sendMoveConnectionRequestDTO.GameId];
            if (sendMoveConnectionRequestDTO.IsAIFirstMove)
            {
                var boardStateRequestDTOAsAIFirst =
                new BoardStateRequestDTO
                {
                    GameId = sendMoveConnectionRequestDTO.GameId,
                    CutableFigure = default,
                    Player = sendMoveConnectionRequestDTO.Player,
                    From = sendMoveConnectionRequestDTO.From,
                    To = sendMoveConnectionRequestDTO.To,
                    GameState = gameState,
                    OpponentColor =
                        sendMoveConnectionRequestDTO.MyColor == FigureColors.Black
                            ? FigureColors.White
                            : FigureColors.Black,
                    IsReadyToEvent = IsReady.IsReadyToMove,
                    IsOpponentComputer = sendMoveConnectionRequestDTO.IsOpponentComputer,
                };

                var moveLogicCommandHandlerAsAIFirst =
                    new MoveLogicCommand<BoardStateRequestDTO,
                        ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>(boardStateRequestDTOAsAIFirst);
                var moveCommandResponse = await mediator.Send(moveLogicCommandHandlerAsAIFirst);
                return moveCommandResponse.IsSuccess
                    ? ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        moveCommandResponse.Data,
                        moveCommandResponse.Message,
                        moveCommandResponse.HttpStatusCode)
                    : ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        moveCommandResponse.Data,
                        moveCommandResponse.Message,
                        moveCommandResponse.HttpStatusCode);

            }
            ;

            var currentPositionBlock = gameState.GetBlockByPosition(sendMoveConnectionRequestDTO.CurrentPosition);

            if (currentPositionBlock.EventColor != EventColors.Cut && currentPositionBlock.EventColor != EventColors.Move && currentPositionBlock.EventColor != EventColors.Castle)
                return invalidResponse;

            var boardStateRequestDTO =
                new BoardStateRequestDTO
                {

                    GameId = sendMoveConnectionRequestDTO.GameId,
                    CutableFigure = default,
                    Player = sendMoveConnectionRequestDTO.Player,
                    From = sendMoveConnectionRequestDTO.From,
                    To = sendMoveConnectionRequestDTO.To,
                    GameState = gameState,
                    OpponentColor =
                        sendMoveConnectionRequestDTO.MyColor == FigureColors.Black
                            ? FigureColors.White
                            : FigureColors.Black,
                    IsReadyToEvent =
                        currentPositionBlock.EventColor == EventColors.Move
                            ? IsReady.IsReadyToMove :
                        currentPositionBlock.EventColor == EventColors.Cut
                            ? IsReady.IsReadyToCut :
                        currentPositionBlock.EventColor == EventColors.Castle
                            ? IsReady.IsReadyToCastle :
                        IsReady.None,
                    IsOpponentComputer = sendMoveConnectionRequestDTO.IsOpponentComputer,
                };

            var moveLogicCommandHandler =
                new MoveLogicCommand<BoardStateRequestDTO,
                    ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>(boardStateRequestDTO);

            if (boardStateRequestDTO.IsReadyToEvent == IsReady.None)
                return invalidResponse;
            else
            {
                var moveCommandResponse = await mediator.Send(moveLogicCommandHandler);
                return moveCommandResponse.IsSuccess
                    ? ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        moveCommandResponse.Data,
                        moveCommandResponse.Message,
                        moveCommandResponse.HttpStatusCode)
                    : ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        moveCommandResponse.Data,
                        moveCommandResponse.Message,
                        moveCommandResponse.HttpStatusCode);
            }
        }

        public async Task<ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(
            ClickRequestDTO sendClickConnectionRequestDTO)
        {
            var validationResult = (await validationService.ValidateAsync(sendClickConnectionRequestDTO));
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(ClickResponseDTO)))!;


            var gameState = ActiveGames.ActiveGamesAndBoards[sendClickConnectionRequestDTO.GameId];

            var currentPositionBlock = gameState.GetBlockByPosition(sendClickConnectionRequestDTO.CurrentPosition);


            var requestDTO = new CanClickRequestDTO
            {
                ClickedBlockInformationDto = sendClickConnectionRequestDTO.PreviusBlockInformationDTO,
                CurrentBlock = currentPositionBlock,
                CurrentBoardBoardState = gameState,
                FigureColor = sendClickConnectionRequestDTO.MyColor
            };
            var sendClickQuery = new SendClickQuery<
                CanClickRequestDTO,
                ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>>(requestDTO);

            gameState.ResetEventableBlocks();

            var canClickResponse = await mediator.Send(sendClickQuery);

            if (!canClickResponse.IsSuccess)
                return ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new ClickResponseDTO()
                    {
                        GameId = sendClickConnectionRequestDTO.GameId,
                        Player = sendClickConnectionRequestDTO.Player
                    },
                    ChessGameResponseMessage.InvalidMove,
                    System.Net.HttpStatusCode.BadRequest);


            var positions =
                gameState.GetBlockByPosition(sendClickConnectionRequestDTO.From).Figure
                    .GetMovableAndCuttableBlocks(sendClickConnectionRequestDTO.From, gameState);

            return ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new ClickResponseDTO()
                {
                    CastlingInfosDTOs = canClickResponse.Data.CastlingInfosDTO!,
                    CutableBlocks = positions.CutableBlock,
                    MovableBlocks = positions.MovableBlock,
                    GameId = sendClickConnectionRequestDTO.GameId,
                    Player = sendClickConnectionRequestDTO.Player,

                },
                ChessGameResponseMessage.SuccessUserConnections,
                System.Net.HttpStatusCode.OK);
        }
    }
}
