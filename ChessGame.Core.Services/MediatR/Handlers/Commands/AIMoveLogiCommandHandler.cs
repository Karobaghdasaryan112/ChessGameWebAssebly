using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Extentions;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Commands
{
    public class AIMoveLogiCommandHandler(
        IMediator mediatR,
        IConnectionService connectionService,
        IValidator<AIMoveLogicRequestDTO> validator,
        ILogger<AIMoveLogiCommandHandler> logger,
        IBoardService boardService) :
        MediatR_Base<AIMoveLogicRequestDTO, AIMoveLogiCommandHandler, IBoardService>
        (validator, logger, boardService),
        IRequestHandler<
            AIMoveLogicCommand<
                AIMoveLogicRequestDTO, ResponseDTO<AIMoveLogicResponseDTO,
                    ChessGameResponseMessage>>,
            ResponseDTO<AIMoveLogicResponseDTO, ChessGameResponseMessage>>
    {
        public async Task<ResponseDTO<AIMoveLogicResponseDTO, ChessGameResponseMessage>>
            Handle(
                AIMoveLogicCommand<AIMoveLogicRequestDTO, ResponseDTO<AIMoveLogicResponseDTO, ChessGameResponseMessage>>
                    request,
                CancellationToken cancellationToken)
        {
            //AI Move Logic Here
            var aiMoveCommand =
                new GetOptimizedMoveQuery<GetOptimizedMoveRequestDTO,
                    ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>>(
                    new GetOptimizedMoveRequestDTO()
                    {
                        GameId = request.RequestDTO.BoardRequestDTO.GameId,
                        ChosenColor = (FigureColors)request.RequestDTO.BoardRequestDTO.GameState.Turn,
                    });

            var mediatRAiMoveResponse = await mediatR.Send(aiMoveCommand, cancellationToken);


            var submitMoveAiCommand =
                new SubmitMoveCommand<SubmitMoveRequestDTO,
                    ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>>(
                    new SubmitMoveRequestDTO()
                    {
                        From = mediatRAiMoveResponse.Data.FromPosition,
                        To = mediatRAiMoveResponse.Data.ToPosition,
                        CurrentBoardState = request.RequestDTO.BoardRequestDTO.GameState,
                        GameId = request.RequestDTO.BoardRequestDTO.GameId
                    });

            var toFigure =
                request.RequestDTO.BoardRequestDTO.GameState.GetBlockByPosition(mediatRAiMoveResponse.Data.ToPosition);

            var toClone = (Block)toFigure.Clone();

            //Submit AI Move via MediatR Command
            var mediatRSubmitAiMoveResponse = await mediatR.Send(submitMoveAiCommand, cancellationToken);

            if (!mediatRSubmitAiMoveResponse.IsSuccess)
                return ResponseDTO<AIMoveLogicResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(null!,
                    ChessGameResponseMessage.InvalidMove, HttpStatusCode.BadRequest);


            //Save Positions after AI Move
            var saveAiPositionsResponse = await boardService.SavePositionsAsync(
                new SavePositionsRequestDTO()
                {
                    FEN = request.RequestDTO.BoardRequestDTO.GameState.FromBoardToFen(),
                    GameId = request.RequestDTO.BoardRequestDTO.GameId,
                });

            //If Saving Positions Fails, return Error Response
            if (!saveAiPositionsResponse.IsSuccess)
                return
                    ResponseDTO<AIMoveLogicResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        new AIMoveLogicResponseDTO()
                        {
                            MoveResponseDTO = new MoveResponseDTO()
                            {
                                GameId = request.RequestDTO.BoardRequestDTO.GameId,
                                Player = request.RequestDTO.BoardRequestDTO.Player
                            },
                        },
                        ChessGameResponseMessage.InternalServerError, HttpStatusCode.InternalServerError);

            request.RequestDTO.BoardRequestDTO.GameState.SwitchTurn();

            var clientBoardStateAfterAiMove = new BoardStateRequestDTO()
            {
                GameId = request.RequestDTO.BoardRequestDTO.GameId,
                Player = request.RequestDTO.BoardRequestDTO.Player,
                GameState = request.RequestDTO.BoardRequestDTO.GameState,
                From = mediatRAiMoveResponse.Data.FromPosition,
                To = mediatRAiMoveResponse.Data.ToPosition,
                OpponentColor = (FigureColors)request.RequestDTO.BoardRequestDTO.GameState.Turn,
                CutableFigure = toClone,
                IsReadyToEvent = toClone.Figure == default ? IsReady.IsReadyToMove : IsReady.IsReadyToCut,
                IsOpponentComputer = request.RequestDTO.BoardRequestDTO.IsOpponentComputer,
            };

            var isKingCheckAfterAIMoveQuery =
                new IsKingCheckedQuery<IsKingCheckedRequestDTO,
                    ResponseDTO<IsKingCheckedResponseDTO, ChessGameResponseMessage>>(
                    new IsKingCheckedRequestDTO()
                    {
                        ChosenColor = request.RequestDTO.BoardRequestDTO.GameState.Turn,
                        CurrentBoard = request.RequestDTO.BoardRequestDTO.GameState,
                    });

            //Check if Opponent King is in Check after AI Move
            var mediatRIsKingCheckAfterAIMove = await mediatR.Send(isKingCheckAfterAIMoveQuery, cancellationToken);

            if (mediatRIsKingCheckAfterAIMove is { Data.IsKingChecked: true })
            {
                var data = new IsKingMateRequestDTO()
                {
                    ChosenColor = request.RequestDTO.BoardRequestDTO.GameState.Turn,
                    CurrentBoard = request.RequestDTO.BoardRequestDTO.GameState,
                    GameId = request.RequestDTO.BoardRequestDTO.GameId
                };

                var checkedKingForOpponent =
                    request.RequestDTO.BoardRequestDTO.GameState.GetBlockByFigureTypeAndColor(FigureType.King,
                        (FigureColors)request.RequestDTO.BoardRequestDTO.GameState.Turn);

                clientBoardStateAfterAiMove.CheckedKingPosition = checkedKingForOpponent.First().Position;

                var isKingMateStateRequest =
                    await mediatR.Send(
                        new IsKingMateQuery<IsKingMateRequestDTO,
                            ResponseDTO<IsKingMateResponseDTO, ChessGameResponseMessage>>(data), cancellationToken);

                var kingMateCommand =
                    new KingMateLogicCommand<KingMateLogicRequestDTO,
                        ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>(
                        new KingMateLogicRequestDTO()
                        {
                            boardStateRequestDTO = clientBoardStateAfterAiMove,
                            isComputerWin = true,
                            IsTrainingGame = true
                        });
                if (isKingMateStateRequest is { IsSuccess: true, Data.IsKingMate: true })
                {
                    var kingMateLogicResponse = await mediatR.Send(kingMateCommand, cancellationToken);
                    
                    await connectionService.SendBoardStateToClient(new BoardStateSenderRequestDTO
                    {
                        connectionId = null,
                        BoardStateRequestDTO = clientBoardStateAfterAiMove,
                        Player = request.RequestDTO.BoardRequestDTO.Player,
                        IsMyConnection = true,
                        Win = false
                    });

                    return ResponseDTO<AIMoveLogicResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        new AIMoveLogicResponseDTO()
                        {
                            MoveResponseDTO = new MoveResponseDTO()
                            {
                                IsReadyToEvent = kingMateLogicResponse.Data.IsReadyToEvent,
                                Player = request.RequestDTO.BoardRequestDTO.Player,
                                GameId = request.RequestDTO.BoardRequestDTO.GameId,
                            },
                        }, ChessGameResponseMessage.GameOver,
                        HttpStatusCode.OK);
                }

                clientBoardStateAfterAiMove.IsKingChecked = true;
            }

            var boardStateSenderReqeust = new BoardStateSenderRequestDTO
            {
                BoardStateRequestDTO = clientBoardStateAfterAiMove,
                Player = request.RequestDTO.BoardRequestDTO.Player,
                IsMyConnection = false,
            };

            await connectionService.SendBoardStateToClient(boardStateSenderReqeust);

            return ResponseDTO<AIMoveLogicResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new AIMoveLogicResponseDTO()
                {
                    MoveResponseDTO = new MoveResponseDTO()
                    {
                        IsReadyToEvent = IsReady.IsReadyToMove,
                        Player = request.RequestDTO.BoardRequestDTO.Player,
                        GameId = request.RequestDTO.BoardRequestDTO.GameId,
                    },
                },
                ChessGameResponseMessage.MoveSuccessful,
                HttpStatusCode.OK);
        }
    }
}