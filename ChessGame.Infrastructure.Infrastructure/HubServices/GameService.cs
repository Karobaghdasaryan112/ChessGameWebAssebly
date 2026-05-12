using System.Net;
using ChessGame.Core.Services.Constants;
using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Extentions;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using MediatR;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Enums.Orientations;
using SharedResources.ChessGameResource.Models;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;
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
        public Task ClearGameAsync(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<PipeLineResponse<GetOnlinePlayersResponseDTO>>
            GetOnlinePlayersAsync(PipeLineRequest<GetONlinePlayersRequestDTO> connectionRequestDTO)
        {
            var pipeLineResponse = new PipeLineResponse<GetOnlinePlayersResponseDTO>();

            var onlinePlayers = connectionService.CurrentConnectionState
                .Where(connectionKeyValuePair => connectionKeyValuePair.Key != connectionRequestDTO.Request.UserGuid)
                .ToDictionary();

            if (onlinePlayers.Count == 0)
            {
                pipeLineResponse.Response =
                    ResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        new()
                        {
                            OnlinePlayers = new()
                        }!,
                        ChessGameResponseMessage.UserConnectionNotFound,
                        System.Net.HttpStatusCode.BadRequest, null!);
                return pipeLineResponse;
            }

            pipeLineResponse.Response =
                ResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>
                    .CreateSuccessResponse(
                        new GetOnlinePlayersResponseDTO() { OnlinePlayers = onlinePlayers },
                        ChessGameResponseMessage.UserConnectionFoundSuccess,
                        System.Net.HttpStatusCode.OK);

            return pipeLineResponse;
        }

        public async Task<PipeLineResponse<TrainingGameResponseDTO>> RequestTrainingGameAsync(
            PipeLineRequest<TrainingGameRequestDTO> trainingGameRequestDTO)
        {
            var GameId = Guid.NewGuid();

            HelperConstants.MAX_DEPTH = (int)trainingGameRequestDTO.Request.TrainingDifficulty;

            var playerGuid = trainingGameRequestDTO.Request.Player1Guid == Guid.Empty
                ? trainingGameRequestDTO.Request.Player2Guid
                : trainingGameRequestDTO.Request.Player1Guid;

            var playerName = trainingGameRequestDTO.Request.Player1Guid == Guid.Empty
                ? trainingGameRequestDTO.Request.Player2Name
                : trainingGameRequestDTO.Request.Player1Name;


            var boardInitializeRequest =
                new SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs.BoardInitializeRequestDTO
                {
                    GameEvent = GameEvent.Training,
                    Player1Name = trainingGameRequestDTO.Request.Player1Name,
                    Player2Name = trainingGameRequestDTO.Request.Player2Name,
                    Player1Id = trainingGameRequestDTO.Request.Player1Guid,
                    Player2Id = trainingGameRequestDTO.Request.Player2Guid,
                    Player1Time = TimeSpan.FromMinutes((int)PlayEvent.Classical),
                    Player2Time = TimeSpan.FromMinutes((int)PlayEvent.Classical),
                };

            var gameState =
                await boardService.InitializeBoardAsync(boardInitializeRequest);


            if (!gameState.IsSuccess)
            {
                return
                    new PipeLineResponse<TrainingGameResponseDTO>()
                    {
                        Response = ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                            new TrainingGameResponseDTO()
                            {
                                GameId = gameState.Data.GameId,
                            },
                            ChessGameResponseMessage.GameCreationFailed,
                            System.Net.HttpStatusCode.InternalServerError)
                    };
            }

            gameState.Data.board = new Board(default(FigureColors));

            connectionService.CurrentConnectionState.TryAdd(
                playerGuid, new UserConnectionDTO()
                {
                    ConnectionId = trainingGameRequestDTO.Request.connectionId,
                    GameId = gameState.Data.GameId,
                    UserName = playerName,
                    Gameinfo =
                        new Gameinfo()
                        {
                            Players = new KeyValuePair<Guid, Guid>(trainingGameRequestDTO.Request.Player1Guid,
                                trainingGameRequestDTO.Request.Player2Guid)
                        }
                });

            ActiveGames.ActiveGamesAndBoards.TryAdd(gameState.Data.GameId, gameState.Data.board);
            var boardStateResponseDTO =
                new BoardStateRequestDTO
                {
                    GameId = gameState.Data.GameId,
                    GameState = gameState.Data.board,
                };
            boardStateResponseDTO.IsOpponentComputer = true;

            var boardStateSenderRequest = new BoardStateSenderRequestDTO
            {
                connectionId = null!,
                BoardStateRequestDTO = boardStateResponseDTO,
                Player = trainingGameRequestDTO.Request.Player1Guid == Guid.Empty
                    ? trainingGameRequestDTO.Request.Player2Name
                    : trainingGameRequestDTO.Request.Player1Name,
                IsMyConnection = true,
            };

            await connectionService.SendBoardStateToClient(boardStateSenderRequest);

            return await Task.FromResult(
                new PipeLineResponse<TrainingGameResponseDTO>()
                {
                    Response = ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        new TrainingGameResponseDTO()
                        {
                            GameId = gameState.Data.GameId,
                            Board = gameState.Data.board
                        },
                        ChessGameResponseMessage.GameCreated,
                        System.Net.HttpStatusCode.OK)
                });
        }


        public async Task<PipeLineResponse<SendGameStateResponseDTO>> SendGameStateAsync(
            PipeLineRequest<SendGameStateReqeustDTO> gameStateReqeustDTO)
        {
            ActiveGames.ActiveGamesAndBoards.TryGetValue(gameStateReqeustDTO.Request.GameId, out var gameState);

            return await Task.FromResult(new PipeLineResponse<SendGameStateResponseDTO>()
            {
                Response = ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    new SendGameStateResponseDTO()
                    {
                        Board = gameState
                    },
                    ChessGameResponseMessage.GameCreated, HttpStatusCode.Created)
            });
        }

        public async Task<PipeLineResponse<SameFigureResposneDTO>> SendIsSameFigureClickedAsync(
            PipeLineRequest<SameFigureRequest> sameFigureRequest)
        {
            var gameState =
                ActiveGames.ActiveGamesAndBoards[sameFigureRequest.Request.GameId];

            var currentPositionBlock = gameState?.GetBlockByPosition(sameFigureRequest.Request.Current);
            var selectedPositionBlock = gameState?.GetBlockByPosition(sameFigureRequest.Request.Selected);
            return await Task.FromResult(
                new PipeLineResponse<SameFigureResposneDTO>
                {
                    Response = ResponseDTO<SameFigureResposneDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        new SameFigureResposneDTO
                        {
                            IsSameFigure = (currentPositionBlock?.Figure?.FigureColor ==
                                            selectedPositionBlock?.Figure?.FigureColor)
                        }, ChessGameResponseMessage.SuccessData, HttpStatusCode.OK)
                });
        }

        public async Task<PipeLineResponse<MoveResponseDTO>> SendMoveAsync(
            PipeLineRequest<MoveRequestDTO> sendMoveConnectionRequestDto)
        {
            var pipelineResponse = new PipeLineResponse<MoveResponseDTO>();
            var data = sendMoveConnectionRequestDto.Request;

            var invalidResponse = ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                new MoveResponseDTO()
                {
                    GameId = data.GameId,
                    Player = data.Player
                },
                ChessGameResponseMessage.InvalidMove,
                System.Net.HttpStatusCode.BadRequest);


            var gameState = ActiveGames.ActiveGamesAndBoards[data.GameId];
            if (data.IsAIFirstMove)
            {
                var boardStateRequestDtoAsAiFirst =
                    new BoardStateRequestDTO
                    {
                        GameId = data.GameId,
                        CutableFigure = null,
                        Player = data.Player,
                        From = data.From,
                        To = data.To,
                        GameState = gameState,
                        OpponentColor =
                            data.MyColor == FigureColors.Black
                                ? FigureColors.White
                                : FigureColors.Black,
                        IsReadyToEvent = IsReady.IsReadyToMove,
                        IsOpponentComputer = data.IsOpponentComputer,
                    };

                var moveLogicCommandHandlerAsAiFirst =
                    new MoveLogicCommand<BoardStateRequestDTO,
                        ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>(boardStateRequestDtoAsAiFirst);
                var moveCommandResponse = await mediator.Send(moveLogicCommandHandlerAsAiFirst);

                pipelineResponse.Response = moveCommandResponse.IsSuccess
                    ? ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        moveCommandResponse.Data,
                        moveCommandResponse.Message,
                        moveCommandResponse.HttpStatusCode)
                    : ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        moveCommandResponse.Data,
                        moveCommandResponse.Message,
                        moveCommandResponse.HttpStatusCode);

                return pipelineResponse;
            }


            var currentPositionBlock = gameState.GetBlockByPosition(data.CurrentPosition);

            // if (currentPositionBlock.EventColor != EventColors.Cut &&
            //     currentPositionBlock.EventColor != EventColors.Move &&
            //     currentPositionBlock.EventColor != EventColors.Castle)
            // {
            //     pipelineResponse.Response = invalidResponse;
            //     return pipelineResponse;
            // }

            var boardStateRequestDto =
                new BoardStateRequestDTO
                {
                    GameId = data.GameId,
                    CutableFigure = null,
                    Player = data.Player,
                    From = data.From,
                    To = data.To,
                    GameState = gameState,
                    OpponentColor =
                        data.MyColor == FigureColors.Black
                            ? FigureColors.White
                            : FigureColors.Black,
                    IsReadyToEvent =
                        currentPositionBlock.EventColor == EventColors.Move
                            ? IsReady.IsReadyToMove
                            : currentPositionBlock.EventColor == EventColors.Cut
                                ? IsReady.IsReadyToCut
                                : currentPositionBlock.EventColor == EventColors.Castle
                                    ? IsReady.IsReadyToCastle
                                    : IsReady.None,
                    IsOpponentComputer = data.IsOpponentComputer,
                };

            var moveLogicCommandHandler =
                new MoveLogicCommand<BoardStateRequestDTO,
                    ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>(boardStateRequestDto);

            if (boardStateRequestDto.IsReadyToEvent == IsReady.None)
            {
                pipelineResponse.Response = invalidResponse;
            }
            else
            {
                var moveCommandResponse = await mediator.Send(moveLogicCommandHandler);

                pipelineResponse.Response = moveCommandResponse.IsSuccess
                    ? ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        moveCommandResponse.Data,
                        moveCommandResponse.Message,
                        moveCommandResponse.HttpStatusCode)
                    : ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        moveCommandResponse.Data,
                        moveCommandResponse.Message,
                        moveCommandResponse.HttpStatusCode);
            }

            return pipelineResponse;
        }

        public async Task<PipeLineResponse<ClickResponseDTO>> SendClickAsync(
            PipeLineRequest<ClickRequestDTO> sendClickConnectionRequestDTO)
        {
            var data = sendClickConnectionRequestDTO.Request;

            var gameState = ActiveGames.ActiveGamesAndBoards[data.GameId];

            var currentPositionBlock = gameState.GetBlockByPosition(data.CurrentPosition);


            var requestDTO = new CanClickRequestDTO
            {
                ClickedBlockInformationDto = data.PreviusBlockInformationDTO,
                CurrentBlock = currentPositionBlock,
                CurrentBoardBoardState = gameState,
                FigureColor = data.MyColor
            };

            var sendClickQuery = new SendClickQuery<
                CanClickRequestDTO,
                ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>>(requestDTO);

            gameState.ResetEventableBlocks();

            var canClickResponse = await CanCLickimpl(requestDTO);

            if (!canClickResponse.IsSuccess)
                return
                    new PipeLineResponse<ClickResponseDTO>()
                    {
                        Response = ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                            new ClickResponseDTO()
                            {
                                GameId = data.GameId,
                                Player = data.Player
                            },
                            ChessGameResponseMessage.InvalidMove,
                            System.Net.HttpStatusCode.BadRequest)
                    };


            var positions =
                gameState.GetBlockByPosition(data.From).Figure
                    .GetMovableAndCuttableBlocks(data.From, gameState);

            return
                new PipeLineResponse<ClickResponseDTO>()
                {
                    Response = ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        new ClickResponseDTO()
                        {
                            CastlingInfosDTOs = canClickResponse.Data.CastlingInfosDTO!,
                            CutableBlocks = positions.CutableBlock,
                            MovableBlocks = positions.MovableBlock,
                            GameId = data.GameId,
                            Player = data.Player,
                        },
                        ChessGameResponseMessage.SuccessUserConnections,
                        System.Net.HttpStatusCode.OK)
                };
        }

        private async Task<ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>> CanCLickimpl(
            CanClickRequestDTO request)
        {
            var figureColor = request.FigureColor;
            var currentBlock = request.CurrentBlock;
            var previusBlockInformationDTO = request.ClickedBlockInformationDto;
            var currentBoard = request.CurrentBoardBoardState!;

            var successResponse = ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new CanClickResponseDTO() { CastlingInfosDTO = new List<CastlingInfosDTO>() },
                ChessGameResponseMessage.MoveSuccessful, HttpStatusCode.Accepted);

            var errorResponse = ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                new CanClickResponseDTO()
                {
                    ClickedBlock = null!
                },
                ChessGameResponseMessage.InvalidMove,
                HttpStatusCode.BadRequest,
                ["It's not the turn of the player"]);

            if ((int)figureColor != (int)currentBoard.Turn)
            {
                // logger.LogWarning("It's not the turn of player with color {Color}", figureColor);

                return errorResponse;
            }

            var currentBlockFromServer = currentBoard.GetBlockByPosition(currentBlock!.Position);

            if (currentBlock.Figure != null && currentBlock.Figure.FigureColor == figureColor)
            {
                // logger.LogInformation("Player with color {Color} clicked on their own figure at position {Position}", figureColor, currentBlock.Position);

                var castlingResult = await GetCastlingMovesAsync(currentBlockFromServer, figureColor, currentBoard);
                if (castlingResult.Any(c => c.IsCastling))
                {
                    // logger.LogInformation("Player with color {Color} is attempting to castle from {FromPosition} to {ToPosition}", figureColor, previusBlockInformationDTO.ClickedPosition, currentBlock.Position);
                    successResponse.Data.ClickedBlock = currentBlockFromServer;
                    successResponse.Data.CastlingInfosDTO = castlingResult;
                    return successResponse;
                }

                successResponse.Data.ClickedBlock = currentBlockFromServer;

                return successResponse;
            }

            if (previusBlockInformationDTO?.ClickedPosition == null ||
                (currentBlockFromServer.EventColor != EventColors.Cut &&
                 currentBlockFromServer.EventColor != EventColors.Move))
                return errorResponse;


            // logger.LogInformation("Player with color {Color} is attempting to move from {FromPosition} to {ToPosition}", figureColor, previusBlockInformationDTO.ClickedPosition, currentBlock.Position);

            successResponse.Data.ClickedBlock = currentBlockFromServer;

            return successResponse;
        }


        private const int ShortCastleDistance = 3;
        private const int LongCastleDistance = 4;

        private async Task<List<CastlingInfosDTO>> GetCastlingMovesAsync(
            Block kingBlock,
            FigureColors color,
            Board board)
        {
            var result = new List<CastlingInfosDTO>();

            if (!CanKingCastle(kingBlock))
                return result;

            var rooks = board.GetBlockByFigureTypeAndColor(FigureType.Rook, color);

            foreach (var rookBlock in rooks)
            {
                if (!CanCastleWithRook(kingBlock, rookBlock))
                    continue;

                if (!IsPathClear(board, kingBlock.Position, rookBlock.Position))
                    continue;

                var isKingSafe = await IsKingSafeDuringCastlingAsync(
                    board,
                    color,
                    kingBlock.Position,
                    rookBlock.Position);

                if (!isKingSafe)
                    continue;

                var isShortCastle =
                    rookBlock.Position.HorizontalOrientation >
                    kingBlock.Position.HorizontalOrientation;

                result.Add(CreateCastlingInfo(
                    kingBlock.Position,
                    isShortCastle));
            }

            HighlightCastlingMoves(board, result);

            return result;
        }

        private static bool CanKingCastle(Block kingBlock)
        {
            return kingBlock.Figure?.FigureType == FigureType.King
                   && !kingBlock.Figure.IsMoves;
        }

        private static bool CanCastleWithRook(Block kingBlock, Block rookBlock)
        {
            if (rookBlock.Figure == null || rookBlock.Figure.IsMoves)
                return false;

            var distance = Math.Abs(
                rookBlock.Position.HorizontalOrientation -
                kingBlock.Position.HorizontalOrientation);

            return distance is ShortCastleDistance or LongCastleDistance;
        }

        private bool IsPathClear(
            Board board,
            Position kingPosition,
            Position rookPosition)
        {
            int step = GetDirection(kingPosition, rookPosition);

            for (int col = (int)kingPosition.HorizontalOrientation + step;
                 col != (int)rookPosition.HorizontalOrientation;
                 col += step)
            {
                var position = new Position(
                    kingPosition.VerticalOrientation,
                    (HorizontalOrientation)col);

                if (board.GetBlockByPosition(position).Figure != null)
                    return false;
            }

            return true;
        }

        private async Task<bool> IsKingSafeDuringCastlingAsync(
            Board board,
            FigureColors color,
            Position kingPosition,
            Position rookPosition)
        {
            int step = GetDirection(kingPosition, rookPosition);

            var simulationBoard = (Board)board.Clone();

            for (int col = (int)kingPosition.HorizontalOrientation + step;
                 col != (int)rookPosition.HorizontalOrientation;
                 col += step)
            {
                var nextPosition = new Position(
                    kingPosition.VerticalOrientation,
                    (HorizontalOrientation)col);

                var moveResult = await SimulateKingMoveAsync(
                    simulationBoard,
                    kingPosition,
                    nextPosition);

                if (!moveResult.IsSuccess || moveResult.Data.IsKingChecked)
                    return false;

                kingPosition = nextPosition;
            }

            return true;
        }

        private async Task<ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>>
            SimulateKingMoveAsync(
                Board board,
                Position from,
                Position to)
        {
            var command =
                new SubmitMoveCommand
                <SubmitMoveRequestDTO,
                    ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>>
                (
                    new SubmitMoveRequestDTO
                    {
                        From = from,
                        To = to,
                        CurrentBoardState = board,
                        GameId = Guid.Empty
                    });

            return await mediator.Send(command);
        }

        private static CastlingInfosDTO CreateCastlingInfo(
            Position kingPosition,
            bool isShortCastle)
        {
            return new CastlingInfosDTO
            {
                IsCastling = true,
                IsShortCastle = isShortCastle,
                CastlingPosition = new Position(
                    kingPosition.VerticalOrientation,
                    isShortCastle
                        ? HorizontalOrientation.g
                        : HorizontalOrientation.c)
            };
        }

        private static void HighlightCastlingMoves(
            Board board,
            IEnumerable<CastlingInfosDTO> castlings)
        {
            foreach (var castling in castlings)
            {
                var block = board.GetBlockByPosition(
                    castling.CastlingPosition);

                block.EventColor = EventColors.Castle;
            }
        }

        private static int GetDirection(
            Position from,
            Position to)
        {
            return to.HorizontalOrientation >
                   from.HorizontalOrientation
                ? 1
                : -1;
        }
    }
}