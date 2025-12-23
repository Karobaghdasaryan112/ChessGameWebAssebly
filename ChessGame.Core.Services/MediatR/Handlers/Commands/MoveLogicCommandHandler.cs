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
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests;
using SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;
using System.Net;
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
                    BoardStateRequestDTO,
                    ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>,
                ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>
    {


        /// <summary>
        /// Processes a chess move request, validates the move, updates the game state, and returns the result of the
        /// move operation.
        /// </summary>
        /// <remarks>This method handles the full lifecycle of a chess move, including move validation,
        /// board state updates, and communication with connected clients. It also manages special game states such as
        /// check and checkmate, and ensures that the game state is persisted after a successful move. If the move
        /// results in check or checkmate, appropriate notifications are sent to clients and the game may be
        /// concluded.</remarks>
        /// <param name="request">The move logic command containing the details of the move to process, including the current board state and
        /// move coordinates.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A response object containing the result of the move operation, including move details and a status message.
        /// Returns an error response if the move is invalid or if an internal error occurs.</returns>
        public async Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> Handle(
                MoveLogicCommand<BoardStateRequestDTO,
                ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var submitMoveCommand = new SubmitMoveCommand<SubmitMoveRequestDTO, ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>>(
                new SubmitMoveRequestDTO()
                {
                    From = request.Request.From,
                    To = request.Request.To,
                    CurrentBoardState = request.Request.GameState,
                    GameId = request.Request.GameId
                });


            //Submit Move via MediatR Command
            var mediatRSubmitMoveResponse = await mediator.Send(submitMoveCommand, cancellationToken);

            if (!mediatRSubmitMoveResponse.IsSuccess)
                return ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(null!, ChessGameResponseMessage.InvalidMove, HttpStatusCode.BadRequest);

            //If King is Checked after Move, notify Current Client
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

            request.Request.GameState.SwitchTurn();

            var isKingCheckedCommandAfterMove = new IsKingCheckedQuery<IsKingCheckedRequestDTO, ResponseDTO<IsKingCheckedResponseDTO, ChessGameResponseMessage>>(
                new IsKingCheckedRequestDTO()
                {
                    ChosenColor = request.Request.GameState.Turn,
                    CurrentBoard = request.Request.GameState,
                });

            //Check if Opponent King is in Check after Move
            var mediatRIsKingCheckAfterMove = await mediator.Send(isKingCheckedCommandAfterMove, cancellationToken);

            if (mediatRIsKingCheckAfterMove is { IsSuccess: true, Data.IsKingChecked: true })
            {
                request.Request.IsKingChecked = true;

                var checkedKingForOpponent = request.Request.GameState.GetBlockByFigureTypeAndColor(FigureType.King, (FigureColors)request.Request.GameState.Turn);

                request.Request.CheckedKingPosition = checkedKingForOpponent.First().Position;

                var data = new IsKingMateRequestDTO()
                {
                    ChosenColor = request.Request.GameState.Turn,
                    CurrentBoard = request.Request.GameState,
                    GameId = request.Request.GameId
                };

                var isKingMateStateRequest = await mediator.Send(new IsKingMateQuery<IsKingMateRequestDTO, ResponseDTO<IsKingMateResponseDTO, ChessGameResponseMessage>>(data), cancellationToken);

                if (isKingMateStateRequest is { IsSuccess: true, Data.IsKingMate: true })
                    return await KingMateLogicAsync(request.Request, isKingMateStateRequest);

                await connectionService.SendBoardStateToClient(request.Request, request.Request.Player, false);

                //for current king colorize is not required
                request.Request.IsKingChecked = false;
                await connectionService.SendBoardStateToClient(request.Request, request.Request.Player, true);
            }

            if (mediatRIsKingCheckAfterMove is { IsSuccess: true, Data.IsKingChecked: false })
            {
                await connectionService.SendBoardStateToClient(request.Request, request.Request.Player, true);
                await connectionService.SendBoardStateToClient(request.Request, request.Request.Player, false);
            }

            return ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(new MoveResponseDTO()
            {
                GameId = request.Request.GameId,
                Player = request.Request.Player,
                IsReadyToEvent = IsReady.IsReadyToCut
            },
            ChessGameResponseMessage.MoveSuccessful,
            HttpStatusCode.OK);
        }


        //Private Methods
        private async Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> KingMateLogicAsync(BoardStateRequestDTO boardStateRequestDTO, ResponseDTO<IsKingMateResponseDTO, ChessGameResponseMessage> isKingMateResponse)
        {
            boardStateRequestDTO.IsKingMate = true;

            await connectionService.SendBoardStateToClient(boardStateRequestDTO, boardStateRequestDTO.Player, false, false);

            await connectionService.SendBoardStateToClient(boardStateRequestDTO, boardStateRequestDTO.Player, true, true);

            var removeUsersFromGameRequest =
                new RemoveUserFromGameRequestDTO()
                {
                    GameId = boardStateRequestDTO.GameId,
                };

            var winnerPlayerGuid = connectionService.CurrentConnectionState.Where(connection => connection.Value.UserName == boardStateRequestDTO.Player).First();

            await service.SaveGameEventAndWinnerAsync(
                 new SaveGameEventAndWinnerRequestDTO()
                 {
                     GameId = boardStateRequestDTO.GameId,
                     WinnerPlayerGuid = winnerPlayerGuid.Key
                 });

            await connectionService.RemoveUsersFromGameAsync(removeUsersFromGameRequest);

            ActiveGames.RemoveGame(boardStateRequestDTO.GameId);

            return ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new MoveResponseDTO()
                {
                    GameId = boardStateRequestDTO.GameId,
                    Player = boardStateRequestDTO.Player,
                    IsReadyToEvent = IsReady.IsReadyToCut
                },
                ChessGameResponseMessage.MoveSuccessful,
                System.Net.HttpStatusCode.OK);
        }
        private async Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> KingCheckCurrentClient(BoardStateRequestDTO boardStateRequestDTO)
        {
            var checkedKingForMe = boardStateRequestDTO.GameState.GetBlockByFigureTypeAndColor(FigureType.King, (FigureColors)boardStateRequestDTO.GameState.Turn);

            boardStateRequestDTO.IsKingChecked = true;
            boardStateRequestDTO.CheckedKingPosition = checkedKingForMe.First().Position;
            boardStateRequestDTO.From = null;
            boardStateRequestDTO.To = null;

            await connectionService.SendBoardStateToClient(boardStateRequestDTO, boardStateRequestDTO.Player, true);

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