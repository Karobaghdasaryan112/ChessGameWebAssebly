using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.Services.Validations;
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
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class InvitationService(
        IConnectionService connectionService,
        BaseHubService baseHubService,
        IMediator mediator,
        GenericValidationService validationService)
        : IInvitationService
    {
        public async Task<ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>>
            AcceptInviteAsync(AcceptInvitationRequestDTO acceptInvitationRequest)
        {
            //Validation
            var validationResult = await validationService.ValidateAsync(acceptInvitationRequest);
            if (!validationResult.IsValid)
            {
                var resultValidation =
                    await validationResult.ReturnValidationResult(default(AcceptInvitationResponseDTO));
                return resultValidation;
            }

            var playersInformation = new KeyValuePair<Guid, Guid>(acceptInvitationRequest.inviterUserGuid,
                acceptInvitationRequest.receiverUserGuid);


            var inviterConnectionInformation = await connectionService.GetUserConnection(
                new ConnectionRequestDTO<GetUserConnectionRequestDTO>()
                {
                    Data = new GetUserConnectionRequestDTO() { UserGuid = acceptInvitationRequest.inviterUserGuid }
                });
            var receiverConnectionInformation = await connectionService.GetUserConnection(
                new ConnectionRequestDTO<GetUserConnectionRequestDTO>()
                {
                    Data = new GetUserConnectionRequestDTO()
                    { UserGuid = acceptInvitationRequest.receiverUserGuid }
                });

            if (!inviterConnectionInformation.IsSuccess || !inviterConnectionInformation.IsSuccess)
            {
                inviterConnectionInformation.Errors.AddRange(receiverConnectionInformation.Errors);
                return ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    null!,
                    inviterConnectionInformation.Message,
                    inviterConnectionInformation.HttpStatusCode,
                    inviterConnectionInformation.Errors
                );
            }
            //Get UserInfos From ConnectionService (ConcurrentDictionary<Guid(userGuid),UserConnectionDTO(UserInfo)>)
            var isInviterExists =
                connectionService.CurrentConnectionState.TryGetValue(acceptInvitationRequest.inviterUserGuid,
                    out var inviterUser);
            var isReceiverExists =
                connectionService.CurrentConnectionState.TryGetValue(acceptInvitationRequest.receiverUserGuid,
                    out var receiverUser);
            //TO DO request using players names and times
            if (!isInviterExists || !isReceiverExists)
                ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new AcceptInvitationResponseDTO(),
                    ChessGameResponseMessage.PlayerNotFound,
                    HttpStatusCode.BadRequest,
                    []);

            var playersTime = TimeSpan.FromSeconds((int)PlayEvent.Classical);
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
            //infos getting success
            var result = await mediator.Send(command);

            await baseHubService.AddToGroupAsync(result.Data!.GameId.ToString(),
                inviterConnectionInformation.Data.UserConnectionDTO.ConnectionId!);

            await baseHubService.AddToGroupAsync(result.Data!.GameId.ToString(),
                receiverConnectionInformation.Data.UserConnectionDTO.ConnectionId!);

            await baseHubService.SendAcceptedInviteAsync(
                inviterConnectionInformation.Data.UserConnectionDTO.ConnectionId!,
                inviterConnectionInformation.Data.UserConnectionDTO,
                acceptInvitationRequest.inviterUserGuid,
                receiverConnectionInformation.Data.UserConnectionDTO,
                acceptInvitationRequest.receiverUserGuid,
                result.Data.GameId);




            inviterConnectionInformation.Data.UserConnectionDTO.GameId = result.Data.GameId;
            receiverConnectionInformation.Data.UserConnectionDTO.GameId = result.Data.GameId;

            var InvitationResponseDTO = new AcceptInvitationResponseDTO()
            {
                GameId = result.Data.GameId,
                PlayerOne_UserConnectionResponseDTO = inviterConnectionInformation.Data.UserConnectionDTO,
                PlayerTwo_UserConnectionResponseDTO = receiverConnectionInformation.Data.UserConnectionDTO
            };

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

            return ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                InvitationResponseDTO,
                ChessGameResponseMessage.SuccessInvitation,
                HttpStatusCode.Created);

        }

        public Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
        {
            throw new NotImplementedException();
        }

        public async Task SendInviteAsync(ConnectionRequestDTO<SendInvitationRequestDTO> connectionRequestDTO)
        {
            await baseHubService.SendInviteAsync(connectionRequestDTO);
        }
    }
}
