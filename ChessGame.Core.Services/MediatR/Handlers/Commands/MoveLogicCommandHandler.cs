using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Extentions;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;
using System.Net;
using SubmitMoveResponseDTO =
    SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs.SubmitMoveResponseDTO;

namespace ChessGame.Core.Services.MediatR.Handlers.Commands
{
    public class MoveLogicCommandHandler(
        IMediator mediator,
        IConnectionService connectionService,
        IGameService gameService,
        IValidator<BoardStateRequestDTO> validator,
        ILogger<MoveLogicCommandHandler> logger,
        IBoardService service)
        : MediatR_Base<BoardStateRequestDTO, MoveLogicCommandHandler, IBoardService>(validator, logger, service)
            , IRequestHandler<
                MoveLogicCommand<
                    BoardStateRequestDTO,
                    ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>,
                ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>
    {
        private CancellationToken cancellationToken;


        public async Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> Handle(
            MoveLogicCommand<BoardStateRequestDTO,
                ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            if (
                request.Request.From!.ToString() == request.Request.To!.ToString() &&
                (int)request.Request.To.HorizontalOrientation == -1 &&
                (int)request.Request.From.HorizontalOrientation == -1)
            {
                if (request.Request.IsOpponentComputer)
                {
                    var aiMoveLogicCommand =
                        new AIMoveLogicCommand<AIMoveLogicRequestDTO,
                            ResponseDTO<AIMoveLogicResponseDTO, ChessGameResponseMessage>>(
                            new AIMoveLogicRequestDTO()
                            {
                                BoardRequestDTO = request.Request
                            });
                    var aiMoveResponse = await mediator.Send(aiMoveLogicCommand, cancellationToken);
                    return ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        aiMoveResponse.Data.MoveResponseDTO!, aiMoveResponse.Message, aiMoveResponse.HttpStatusCode);
                }
            }

            var submitMoveCommand =
                new SubmitMoveCommand<SubmitMoveRequestDTO,
                    ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>>(
                    new SubmitMoveRequestDTO()
                    {
                        PromotionFigure = request.Request.PromotionFigure,
                        From = request.Request.From,
                        To = request.Request.To,
                        CurrentBoardState = request.Request.GameState,
                        GameId = request.Request.GameId
                    });

            var mediatRSubmitMoveResponse = await mediator.Send(submitMoveCommand, cancellationToken);

            if (!mediatRSubmitMoveResponse.IsSuccess)
                return ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(null!,
                    ChessGameResponseMessage.InvalidMove, HttpStatusCode.BadRequest);

            if (mediatRSubmitMoveResponse.Data is { IsKingChecked: true })
                return await KingCheckCurrentClient(request.Request);


            //Save Positions after Move
            var savePositionsResponse = await service.SavePositionsAsync(
                new SavePositionsRequestDTO()
                {
                    FEN = request.Request.GameState.FromBoardToFen(),
                    GameId = request.Request.GameId,
                });


            //If Saving Positions Fails, return Error Response
            if (!savePositionsResponse.IsSuccess)
                return
                    ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(new MoveResponseDTO()
                        {
                            GameId = request.Request.GameId,
                            Player = request.Request.Player
                        },
                        ChessGameResponseMessage.InternalServerError, HttpStatusCode.InternalServerError);


            //Swtich turn after move
            request.Request.GameState.SwitchTurn();

            //King Check Command Initializing
            var isKingCheckedCommandAfterMove =
                new IsKingCheckedQuery<IsKingCheckedRequestDTO,
                    ResponseDTO<IsKingCheckedResponseDTO, ChessGameResponseMessage>>(
                    new IsKingCheckedRequestDTO()
                    {
                        GameId = request.Request.GameId,
                        ChosenColor = request.Request.GameState.Turn,
                        CurrentBoard = request.Request.GameState,
                    });

            //Check if Opponent King is in Check after Move
            var mediatRIsKingCheckAfterMove = await mediator.Send(isKingCheckedCommandAfterMove, cancellationToken);

            //King Check Notifier for Opponent Client after Move
            var boardSTateSenderRequest = new BoardStateSenderRequestDTO
            {
                BoardStateRequestDTO = request.Request,
                Player = request.Request.Player,
                IsMyConnection = false,
            };

            if (mediatRIsKingCheckAfterMove is { IsSuccess: true, Data.IsKingChecked: true })
            {
                //Initializing
                InitializeKingLogicRequest(request);

                //IsKingMate Request Data Initializing
                var data = new IsKingMateRequestDTO()
                {
                    ChosenColor = request.Request.GameState.Turn,
                    CurrentBoard = request.Request.GameState,
                    GameId = request.Request.GameId
                };

                var isKingMateStateRequest =
                    await mediator.Send(
                        new IsKingMateQuery<IsKingMateRequestDTO,
                            ResponseDTO<IsKingMateResponseDTO, ChessGameResponseMessage>>(data), cancellationToken);

                //If King Mate State, process King Mate Logic
                //King Mate Logic via MediatR
                if (isKingMateStateRequest is { IsSuccess: true, Data.IsKingMate: true })
                {
                    var kingMateLogicRequestDTO = new KingMateLogicRequestDTO()
                    {
                        boardStateRequestDTO = request.Request,
                        IsTrainingGame = request.Request.IsOpponentComputer,
                    };
                    var kingMateLogicCommand =
                        new KingMateLogicCommand<KingMateLogicRequestDTO,
                            ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>(kingMateLogicRequestDTO);
                    return await mediator.Send(kingMateLogicCommand, cancellationToken);
                }


                await connectionService.SendBoardStateToClient(boardSTateSenderRequest);
            }

            if (mediatRIsKingCheckAfterMove is { IsSuccess: true, Data.IsKingChecked: false })
            {
                await connectionService.SendBoardStateToClient(boardSTateSenderRequest);


                if (mediatRSubmitMoveResponse.Data.CastlingRookPositions != default)
                {
                    var castlingRequest = request.Request;

                    castlingRequest.From = mediatRSubmitMoveResponse.Data.CastlingRookPositions.RookFrom;
                    castlingRequest.To = mediatRSubmitMoveResponse.Data.CastlingRookPositions.RookTo;

                    await connectionService.SendBoardStateToClient(boardSTateSenderRequest);
                }
            }

            //if this Game Playing with AI, here should be AI Move Logic
            //MediatR Call for AI Move
            if (request.Request.IsOpponentComputer)
            {
                var aiMoveLogicCommand =
                    new AIMoveLogicCommand<AIMoveLogicRequestDTO,
                        ResponseDTO<AIMoveLogicResponseDTO, ChessGameResponseMessage>>(
                        new AIMoveLogicRequestDTO()
                        {
                            BoardRequestDTO = request.Request
                        });
                var aiMoveResponse = await mediator.Send(aiMoveLogicCommand, cancellationToken);
                return ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    aiMoveResponse.Data.MoveResponseDTO!, aiMoveResponse.Message, aiMoveResponse.HttpStatusCode);
            }
            //end of AI Move Logic

            return ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(new MoveResponseDTO()
                {
                    GameId = request.Request.GameId,
                    Player = request.Request.Player,
                    IsReadyToEvent = request.Request.IsReadyToEvent
                },
                ChessGameResponseMessage.MoveSuccessful,
                HttpStatusCode.OK);
        }


        //Private Methods
        private MoveLogicCommand<BoardStateRequestDTO, ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>
            InitializeKingLogicRequest(
                MoveLogicCommand<BoardStateRequestDTO, ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>
                    moveLogicCommand)
        {
            moveLogicCommand.Request.IsKingChecked = true;

            var checkedKingForOpponent =
                moveLogicCommand.Request.GameState.GetBlockByFigureTypeAndColor(FigureType.King,
                    (FigureColors)moveLogicCommand.Request.GameState.Turn);

            moveLogicCommand.Request.CheckedKingPosition = checkedKingForOpponent.First().Position;

            return moveLogicCommand;
        }

        private async Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> KingCheckCurrentClient(
            BoardStateRequestDTO boardStateRequestDTO)
        {
            var checkedKingForMe = boardStateRequestDTO.GameState.GetBlockByFigureTypeAndColor(FigureType.King,
                (FigureColors)boardStateRequestDTO.GameState.Turn);

            boardStateRequestDTO.IsKingChecked = true;
            boardStateRequestDTO.CheckedKingPosition = checkedKingForMe.First().Position;
            boardStateRequestDTO.From = null;
            boardStateRequestDTO.To = null;


            var boardStateSenderRequest = new BoardStateSenderRequestDTO
            {
                BoardStateRequestDTO = boardStateRequestDTO,
                Player = boardStateRequestDTO.Player,
                IsMyConnection = true,
            };

            await connectionService.SendBoardStateToClient(boardStateSenderRequest);

            return
                ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(new MoveResponseDTO()
                    {
                        GameId = boardStateRequestDTO.GameId,
                        Player = boardStateRequestDTO.Player
                    },
                    ChessGameResponseMessage.InvalidMove, HttpStatusCode.BadRequest);
        }
    }
}