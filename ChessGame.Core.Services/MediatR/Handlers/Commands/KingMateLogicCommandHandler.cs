using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests;

namespace ChessGame.Core.Services.MediatR.Handlers.Commands
{
    public class KingMateLogicCommandHandler(
        IValidator<KingMateLogicRequestDTO> validator,
        ILogger<KingMateLogicCommandHandler> logger,
        IConnectionService connectionService,
        IBoardService boardService) :
        MediatR_Base<KingMateLogicRequestDTO, KingMateLogicCommandHandler, IBoardService>(validator, logger,
            boardService),
        IRequestHandler<
            KingMateLogicCommand<
                KingMateLogicRequestDTO,
                ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>,
            ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>
    {
        public async Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> Handle(
            KingMateLogicCommand<KingMateLogicRequestDTO, ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>
                request,
            CancellationToken cancellationToken)
        {
            // 1. Extract and Prepare State
            var requestData = request.RequestDTO;
            var boardReq = requestData.boardStateRequestDTO;
            var gameId = boardReq.GameId;

            // Explicitly set Mate state as per business logic
            boardReq.IsKingMate = true;

            // 2. Broadcast Game State
            // We pass the "Winning" status for the current player. 
            // The SendBoardStateToClient service handles flipping this for the opponent.
            var boardSenderRequest = new BoardStateSenderRequestDTO
            {
                BoardStateRequestDTO = boardReq,
                Player = boardReq.Player,
                IsMyConnection = true,
                Win = request.RequestDTO.boardStateRequestDTO.IsOpponentComputer ? !requestData.isComputerWin : null
            };

            // This replaces the old if(IsTrainingGame) block by delegating 
            // the "who gets what" logic to the specialized broadcast service.
            await connectionService.SendBoardStateToClient(boardSenderRequest);

            // 3. Persist Winner Information
            // Replaces .First() with a safe lookup to prevent runtime crashes
            var winnerConnection = connectionService.CurrentConnectionState
                .FirstOrDefault(c => c.Value.UserName == boardReq.Player);

            if (winnerConnection.Value != null)
            {
                await boardService.SaveGameEventAndWinnerAsync(new SaveGameEventAndWinnerRequestDTO
                {
                    GameId = gameId,
                    WinnerPlayerGuid = winnerConnection.Key // The Connection Key is the User Guid
                });
            }

            // 4. Resource Cleanup (Ordered to ensure data integrity)
            var cleanupRequest = new RemoveUserFromGameRequestDTO { GameId = gameId };

            // Remove from SignalR Groups/Connections
            await connectionService.RemoveUsersFromGameAsync(cleanupRequest);

            // Remove from Server Memory
            ActiveGames.RemoveGame(gameId);

            // 5. Build and Return Success Response
            return ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new MoveResponseDTO
                {
                    GameId = gameId,
                    Player = boardReq.Player,
                    IsReadyToEvent = IsReady.IsReadyToCut
                },
                ChessGameResponseMessage.MoveSuccessful,
                System.Net.HttpStatusCode.OK);
        }
    }
}