using System.Net;
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
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Requests;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests;
using SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;
using SubmitMoveResponseDTO = SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs.SubmitMoveResponseDTO;

namespace ChessGame.Core.Services.MediatR.Handlers.Commands
{
    public class MoveLogicCommandHandler(
        IMediator mediator,
        IConnectionService connectionService,
        IValidator<BoardStateRequestDTO> validator,
        ILogger<MoveLogicCommandHandler> logger,
        IBoardService service)
        : MediatR_Base<BoardStateRequestDTO, MoveLogicCommandHandler, IBoardService>(validator, logger, service)
            , IRequestHandler<
                MoveLogicCommand<
                    IRequestTypes<BoardStateRequestDTO>,
                    IResponseTypes<MoveResponseDTO, ChessGameResponseMessage>>,
                IResponseTypes<MoveResponseDTO, ChessGameResponseMessage>>
    {
        public async Task<IResponseTypes<MoveResponseDTO, ChessGameResponseMessage>> Handle(
            MoveLogicCommand<IRequestTypes<BoardStateRequestDTO>,
                IResponseTypes<MoveResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var submitMoveRequest = new ChessGameRequest<SubmitMoveRequestDTO>()
            {
                requestType =
                    new SubmitMoveRequestDTO()
                    {
                        From = request.Request.requestType.From,
                        To = request.Request.requestType.To,
                        CurrentBoardState = request.Request.requestType.GameState,
                        GameId = request.Request.requestType.GameId
                    }
            };
            var submitMoveCommand =
                new SubmitMoveCommand<IRequestTypes<SubmitMoveRequestDTO>,
                    IResponseTypes<SubmitMoveResponseDTO, ChessGameResponseMessage>>(submitMoveRequest);
            var mediatRSubmitMoveResponse = await mediator.Send(submitMoveCommand, cancellationToken);

            request.Request.requestType.GameState.ResetEventableBlocks();

            if (!mediatRSubmitMoveResponse.IsSuccess)
                return ChessGameResponse<MoveResponseDTO>.CreateErrorResponse(
                    ChessGameResponseMessage.InvalidMove,
                    System.Net.HttpStatusCode.BadRequest);

            var saveGameStateRequest = new ConnectionRequestDTO<SavePositionsRequestDTO>()
            {
                Data = new SavePositionsRequestDTO()
                {
                    FEN = request.Request.requestType.GameState.FromBoardToFen(),
                    GameId = request.Request.requestType.GameId,
                }
            };

            var savePositionsResponse = await service.SavePositionsAsync(saveGameStateRequest);
            if (!savePositionsResponse.IsSuccess)
                return ChessGameResponse<MoveResponseDTO>.CreateErrorResponse(new MoveResponseDTO()
                {
                    GameId = request.Request.requestType.GameId,
                    Player = request.Request.requestType.Player
                },
                    ChessGameResponseMessage.InternalServerError,
                    HttpStatusCode.InternalServerError);

            if (mediatRSubmitMoveResponse.Data is { IsKingChecked: true })
            {
                    
                var checkedKingForMe =
                    request.Request.requestType.GameState.GetBlockByFigureTypeAndColor(FigureType.King,
                        (FigureColors)request.Request.requestType.GameState.Turn);
                request.Request.requestType.IsKingChecked = true;
                request.Request.requestType.CheckedKingPosition = checkedKingForMe.First().Position;
                request.Request.requestType.From = null;
                request.Request.requestType.To = null;

                //TO DO
                await connectionService.SendBoardStateToClient(
                    new ConnectionRequestDTO<BoardStateRequestDTO>() { Data = request.Request.requestType },
                    request.Request.requestType.Player, true);

                return ChessGameResponse<MoveResponseDTO>.CreateErrorResponse(new MoveResponseDTO()
                {
                    GameId = request.Request.requestType.GameId,
                    Player = request.Request.requestType.Player
                },
                    ChessGameResponseMessage.InvalidMove,
                    System.Net.HttpStatusCode.BadRequest);

            }

            request.Request.requestType.GameState.SwitchTurn();


            var isKingCheckedAfterMove = new ChessGameRequest<IsKingCheckedRequestDTO>()
            {
                requestType =
                    new IsKingCheckedRequestDTO()
                    {

                        ChosenColor = request.Request.requestType.GameState.Turn,
                        CurrentBoard = request.Request.requestType.GameState,
                    }
            };
            var isKingCheckedCommandAfterMove =
                new IsKingCheckedQuery<IRequestTypes<IsKingCheckedRequestDTO>,
                    IResponseTypes<IsKingCheckedResponseDTO, ChessGameResponseMessage>>(isKingCheckedAfterMove);

            var mediatRIsKingCheckAfterMove = await mediator.Send(isKingCheckedCommandAfterMove, cancellationToken);

            if (mediatRIsKingCheckAfterMove is { IsSuccess: true, Data.IsKingChecked: true })
            {
                request.Request.requestType.IsKingChecked = true;


                var checkedKingForOpponent =
                    request.Request.requestType.GameState.GetBlockByFigureTypeAndColor(FigureType.King,
                        (FigureColors)request.Request.requestType.GameState.Turn);
                request.Request.requestType.CheckedKingPosition = checkedKingForOpponent.First().Position;

                var isKingMateStateQueryRequest =
                    new IsKingMateQuery<IRequestTypes<IsKingMateRequestDTO>,
                        IResponseTypes<IsKingMateResponseDTO, ChessGameResponseMessage>>(
                        new ChessGameRequest<IsKingMateRequestDTO>()
                        {
                            requestType = new IsKingMateRequestDTO()
                            {
                                ChosenColor = request.Request.requestType.GameState.Turn,
                                CurrentBoard = request.Request.requestType.GameState,
                                GameId = request.Request.requestType.GameId
                            }
                        });

                var isKingMateStateRequest = await mediator.Send(isKingMateStateQueryRequest, cancellationToken);

                if (isKingMateStateRequest is { IsSuccess: true, Data.IsKingMate: true })
                {
                    request.Request.requestType.IsKingMate = true;



                    await connectionService.SendBoardStateToClient(
                        new ConnectionRequestDTO<BoardStateRequestDTO>() { Data = request.Request.requestType },
                        request.Request.requestType.Player, false, false);

                    await connectionService.SendBoardStateToClient(
                        new ConnectionRequestDTO<BoardStateRequestDTO>() { Data = request.Request.requestType },
                        request.Request.requestType.Player, true, true);


                    var removeUsersFromGameRequest = new ConnectionRequestDTO<RemoveUserFromGameRequestDTO>()
                    {
                        Data = new RemoveUserFromGameRequestDTO()
                        {
                            GameId = request.Request.requestType.GameId,
                        }
                    };


                    await connectionService.RemoveUsersFromGameAsync(removeUsersFromGameRequest);

                    ActiveGames.RemoveGame(request.Request.requestType.GameId);

                    return ChessGameResponse<MoveResponseDTO>.CreateSuccessResponse(
                        new MoveResponseDTO()
                        {
                            GameId = request.Request.requestType.GameId,
                            Player = request.Request.requestType.Player,
                            IsReadyToEvent = IsReady.IsReadyToCut
                        },
                        ChessGameResponseMessage.MoveSuccessful,
                        System.Net.HttpStatusCode.OK
                        , null!);
                }
            }

            //Opponent Client MyConnection -True
            await connectionService.SendBoardStateToClient(
                new ConnectionRequestDTO<BoardStateRequestDTO>() { Data = request.Request.requestType },
                request.Request.requestType.Player, true);

            //Opponent Client MyConnection -False
            await connectionService.SendBoardStateToClient(
                new ConnectionRequestDTO<BoardStateRequestDTO>() { Data = request.Request.requestType },
                request.Request.requestType.Player, false);

            return ChessGameResponse<MoveResponseDTO>.CreateSuccessResponse(new MoveResponseDTO()
            {
                GameId = request.Request.requestType.GameId,
                Player = request.Request.requestType.Player,
                IsReadyToEvent = IsReady.IsReadyToCut
            },
                ChessGameResponseMessage.MoveSuccessful,
                System.Net.HttpStatusCode.OK, null);
        }
    }
}