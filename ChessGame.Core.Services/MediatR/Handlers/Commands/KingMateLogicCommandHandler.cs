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
        public async Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>
            Handle(
                KingMateLogicCommand<KingMateLogicRequestDTO, ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>
                    request,
                CancellationToken cancellationToken)
        {
            request.RequestDTO.boardStateRequestDTO.IsKingMate = true;

            var boardStateSenderRequestDTO = new BoardStateSenderRequestDTO
            {
                BoardStateRequestDTO = request.RequestDTO.boardStateRequestDTO,
                Player = request.RequestDTO.boardStateRequestDTO.Player,
                IsMyConnection = false,
                Win = !request.RequestDTO.isComputerWin
            };

            if (request.RequestDTO.IsTrainingGame)
            {
                await connectionService.SendBoardStateToClient(boardStateSenderRequestDTO);
            }
            else
            {
                boardStateSenderRequestDTO.Win = false;
                await connectionService.SendBoardStateToClient(boardStateSenderRequestDTO);

                
                boardStateSenderRequestDTO.Win = true;
                boardStateSenderRequestDTO.IsMyConnection = true;
                await connectionService.SendBoardStateToClient(boardStateSenderRequestDTO);
            }

            var removeUsersFromGameRequest =
                new RemoveUserFromGameRequestDTO()
                {
                    GameId = (request.RequestDTO.boardStateRequestDTO.GameId),
                };

            var winnerPlayerGuid = connectionService.CurrentConnectionState.Where(connection =>
            connection.Value.UserName == request.RequestDTO.boardStateRequestDTO.Player)?.First();

            await boardService.SaveGameEventAndWinnerAsync(
                new SaveGameEventAndWinnerRequestDTO()
                {
                    GameId = request.RequestDTO.boardStateRequestDTO.GameId,
                    WinnerPlayerGuid = winnerPlayerGuid!.Value.Key!
                });

            await connectionService.RemoveUsersFromGameAsync(removeUsersFromGameRequest);

            ActiveGames.RemoveGame(request.RequestDTO.boardStateRequestDTO.GameId);

            return ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new MoveResponseDTO()
                {
                    GameId = request.RequestDTO.boardStateRequestDTO.GameId,
                    Player = request.RequestDTO.boardStateRequestDTO.Player,
                    IsReadyToEvent = IsReady.IsReadyToCut
                },
                ChessGameResponseMessage.MoveSuccessful,
                System.Net.HttpStatusCode.OK);
        }
    }
}