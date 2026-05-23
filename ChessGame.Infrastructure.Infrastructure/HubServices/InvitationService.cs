using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using MediatR;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;
using System.Net;
using SharedResources.ChessGameResource.Enums.Colors;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class InvitationService(
        IConnectionService connectionService,
        BaseHubService baseHubService,
        IMediator mediator,
        GenericValidationService validationService)
        : IInvitationService
    {
        public async Task<PipeLineResponse<AcceptInvitationResponseDTO>> AcceptInviteAsync(
            AcceptInvitationRequestDTO acceptInvitationRequest)
        {
            var pipeLineResponse = new PipeLineResponse<AcceptInvitationResponseDTO>();

            var inviterResponse = await connectionService.GetUserConnection(
                new GetUserConnectionRequestDTO()
                {
                    UserGuid = acceptInvitationRequest.inviterUserGuid
                });
            var receiverResponse = await connectionService.GetUserConnection(
                new GetUserConnectionRequestDTO()
                {
                    UserGuid = acceptInvitationRequest.receiverUserGuid
                });


            var receiverConnectionInformation = receiverResponse.Response;
            var inviterConnectionInformation = inviterResponse.Response;


            if (!inviterConnectionInformation.IsSuccess || !receiverConnectionInformation.IsSuccess)
            {
                inviterConnectionInformation.Errors.AddRange(receiverConnectionInformation.Errors);

                pipeLineResponse.Response = ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>
                    .CreateErrorResponse(
                        null!,
                        inviterConnectionInformation.Message,
                        inviterConnectionInformation.HttpStatusCode,
                        inviterConnectionInformation.Errors
                    );
                return pipeLineResponse;
            }

            var isInviterExists =
                connectionService.CurrentConnectionState.TryGetValue(acceptInvitationRequest.inviterUserGuid,
                    out var inviterUser);

            var isReceiverExists =
                connectionService.CurrentConnectionState.TryGetValue(acceptInvitationRequest.receiverUserGuid,
                    out var receiverUser);

            if (!isInviterExists || !isReceiverExists)
            {
                pipeLineResponse.Response = ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>
                    .CreateErrorResponse(
                        new AcceptInvitationResponseDTO(),
                        ChessGameResponseMessage.PlayerNotFound,
                        HttpStatusCode.BadRequest,
                        []);
                return pipeLineResponse;
            }

            var playersTime = TimeSpan.FromSeconds((int)acceptInvitationRequest.PlayEvent);

            var command =
                new BoardInitializeCommand<BoardInitializeRequestDTO,
                    ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>>
                (
                    new BoardInitializeRequestDTO()
                    {
                        GameEvent = GameEvent.Start,
                        Player1Time = playersTime,
                        Player2Time = playersTime,
                        Player1Name = inviterUser?.UserName!,
                        Player2Name = receiverUser?.UserName!,
                        Player1Id = acceptInvitationRequest.inviterUserGuid,
                        Player2Id = acceptInvitationRequest.receiverUserGuid
                    });

            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                pipeLineResponse.Response =
                    ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        null!,
                        result.Message,
                        result.HttpStatusCode,
                        result.Errors
                    );
                return pipeLineResponse;
            }

            inviterConnectionInformation.Data.UserConnectionDTO.GameId = result.Data.GameId;
            receiverConnectionInformation.Data.UserConnectionDTO.GameId = result.Data.GameId;

            var invitationResponseDto = new AcceptInvitationResponseDTO()
            {
                GameId = result.Data.GameId,
                PlayerOne_UserConnectionResponseDTO = inviterConnectionInformation.Data.UserConnectionDTO,
                PlayerTwo_UserConnectionResponseDTO = receiverConnectionInformation.Data.UserConnectionDTO
            };

            ActiveGames._connections[acceptInvitationRequest.receiverUserGuid] =
                receiverConnectionInformation.Data.UserConnectionDTO;

            ActiveGames._connections[acceptInvitationRequest.inviterUserGuid] =
                inviterConnectionInformation.Data.UserConnectionDTO;

            ActiveGames._connections[acceptInvitationRequest.receiverUserGuid].GameId = result.Data.GameId;

            ActiveGames._connections[acceptInvitationRequest.receiverUserGuid].Gameinfo =
                new Gameinfo()
                {
                    Players = new KeyValuePair<Guid, Guid>(
                        acceptInvitationRequest.receiverUserGuid,
                        acceptInvitationRequest.inviterUserGuid)
                };

            ActiveGames._connections[acceptInvitationRequest.inviterUserGuid].GameId = result.Data.GameId;

            ActiveGames._connections[acceptInvitationRequest.inviterUserGuid].Gameinfo =
                new Gameinfo()
                {
                    Players = new KeyValuePair<Guid, Guid>(
                        acceptInvitationRequest.receiverUserGuid,
                        acceptInvitationRequest.inviterUserGuid)
                };


            pipeLineResponse.Response =
                ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    invitationResponseDto,
                    ChessGameResponseMessage.SuccessInvitation,
                    HttpStatusCode.Created);

            await baseHubService.AddToGroupAsync(
                result.Data.GameId.ToString(),
                inviterConnectionInformation.Data.UserConnectionDTO.ConnectionId);

            await baseHubService.AddToGroupAsync(
                result.Data.GameId.ToString(),
                receiverConnectionInformation.Data.UserConnectionDTO.ConnectionId);

            await baseHubService.SendAcceptedInviteAsync(
                inviterConnectionInformation.Data.UserConnectionDTO,
                acceptInvitationRequest.inviterUserGuid,
                receiverConnectionInformation.Data.UserConnectionDTO,
                acceptInvitationRequest.receiverUserGuid,
                result.Data.GameId);

            StartTick(result.Data.GameId,inviterConnectionInformation.Data.UserConnectionDTO.ConnectionId,receiverConnectionInformation.Data.UserConnectionDTO.ConnectionId);

            return pipeLineResponse;
        }

        public Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
        {
            return Task.CompletedTask;
        }

        public async Task<PipeLineResponse<SendInvitationsResponseDTO>> SendInviteAsync(
            SendInvitationRequestDTO connectionRequestDto)
        {
            var inviterResponse = await connectionService.GetUserConnection(
                new GetUserConnectionRequestDTO
                {
                    UserGuid = connectionRequestDto.InviterPlayerId
                });

            var receiverResponse = await connectionService.GetUserConnection(
                new GetUserConnectionRequestDTO
                {
                    UserGuid = connectionRequestDto.ReceiverPlayerId
                });


            var inviterConnectionInfo = inviterResponse.Response;
            var receiverConnectionInfo = receiverResponse.Response;


            if (!inviterConnectionInfo.IsSuccess || !receiverConnectionInfo.IsSuccess)
            {
                return new PipeLineResponse<SendInvitationsResponseDTO>
                {
                    Response = ResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        null!,
                        ChessGameResponseMessage.PlayerNotFound,
                        HttpStatusCode.NotFound,
                        [])
                };
            }

            connectionRequestDto.InviterUserConnection = inviterConnectionInfo.Data.UserConnectionDTO;
            connectionRequestDto.ReceiverUserConnection = receiverConnectionInfo.Data.UserConnectionDTO;


            await baseHubService.SendInviteAsync(connectionRequestDto);
            return new PipeLineResponse<SendInvitationsResponseDTO>()
            {
                Response = ResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    null!, ChessGameResponseMessage.SuccessInvitation, HttpStatusCode.Created, new object()),
            };
        }
        
        
        
        //Private Methods
        private void StartTick(Guid gameId,string inviterUserConnection,string receiverUserConnection)
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(900);

                    if (!ActiveGames.ActiveGamesAndBoards.ContainsKey(gameId)) break;
                    
                    var board = ActiveGames.GetBoard(gameId);
                    
                    var figureColor = board.Turn;
                    
                    
                    if (figureColor == Turn.White)
                        board.WhiteTimeSpan -= TimeSpan.FromSeconds(1); 
                    else
                        board.BlackTimeSpan -= TimeSpan.FromSeconds(1);

                    
                    await baseHubService.ReceiveTickConnection(
                        board.FigureColor,
                        board.WhiteTimeSpan,
                        board.BlackTimeSpan,
                        inviterUserConnection
                    );
                    
                    
                    await baseHubService.ReceiveTickConnection(
                        board.FigureColor,
                        board.WhiteTimeSpan,
                        board.BlackTimeSpan,
                        receiverUserConnection
                    );
                }
            });
        }
        
        
    }
}