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
using ChessGame.Core.Services.PipeLine;
using SharedResources.Contracts;
using SharedResources.DTOs.ErrorResponseDTOs;

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

            //Get the connection information of both players using their userGuids from the connection service
            var inviterConnectionInformation = await connectionService.GetUserConnection(
             new GetUserConnectionRequestDTO()
             {
                 UserGuid = acceptInvitationRequest.inviterUserGuid
             });
            var receiverConnectionInformation = await connectionService.GetUserConnection(
                new GetUserConnectionRequestDTO()
                {
                    UserGuid = acceptInvitationRequest.receiverUserGuid
                });

            //If any of the connection information retrieval is not successful, return an error response with the errors from both retrievals
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

            //Check if both players exist in the current connections
            var isInviterExists =
                connectionService.CurrentConnectionState.TryGetValue(acceptInvitationRequest.inviterUserGuid,
                    out var inviterUser);
            var isReceiverExists =
                connectionService.CurrentConnectionState.TryGetValue(acceptInvitationRequest.receiverUserGuid,
                    out var receiverUser);

            //If any of the players does not exist in the current connections, return an error response
            if (!isInviterExists || !isReceiverExists)
                ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new AcceptInvitationResponseDTO(),
                    ChessGameResponseMessage.PlayerNotFound,
                    HttpStatusCode.BadRequest,
                    []);

            //Initialize the board and create the game with the gameId and the connection information of both players
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

            var result = await mediator.Send(command);

            if (!result.IsSuccess)
                return ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    null!,
                    result.Message,
                    result.HttpStatusCode,
                    result.Errors
                );

            inviterConnectionInformation.Data.UserConnectionDTO.GameId = result.Data.GameId;
            receiverConnectionInformation.Data.UserConnectionDTO.GameId = result.Data.GameId;

            var InvitationResponseDTO = new AcceptInvitationResponseDTO()
            {
                GameId = result.Data.GameId,
                PlayerOne_UserConnectionResponseDTO = inviterConnectionInformation.Data.UserConnectionDTO,
                PlayerTwo_UserConnectionResponseDTO = receiverConnectionInformation.Data.UserConnectionDTO
            };

            //Add the userConnectionDtos of both players to the active connections and add the gameId to their connection information
            ActiveGames._connections[acceptInvitationRequest.receiverUserGuid] = receiverConnectionInformation.Data.UserConnectionDTO;
            ActiveGames._connections[acceptInvitationRequest.inviterUserGuid] = inviterConnectionInformation.Data.UserConnectionDTO;

            //Add the gameId and gameinfo to the active connections of both players
            ActiveGames._connections[acceptInvitationRequest.receiverUserGuid].GameId = result.Data.GameId;
            ActiveGames._connections[acceptInvitationRequest.receiverUserGuid].Gameinfo =
                new Gameinfo()
                {

                    Players = new KeyValuePair<Guid, Guid>(
                        acceptInvitationRequest.receiverUserGuid,
                        acceptInvitationRequest.inviterUserGuid)
                };

            //Add the gameId and gameinfo to the active connections of both players
            ActiveGames._connections[acceptInvitationRequest.inviterUserGuid].GameId = result.Data.GameId;
            ActiveGames._connections[acceptInvitationRequest.inviterUserGuid].Gameinfo =
                new Gameinfo()
                {
                    Players = new KeyValuePair<Guid, Guid>(
                        acceptInvitationRequest.receiverUserGuid,
                        acceptInvitationRequest.inviterUserGuid)
                };

            //Add both players to the group of the gameId
            await baseHubService.AddToGroupAsync(result.Data!.GameId.ToString(),
                inviterConnectionInformation.Data.UserConnectionDTO.ConnectionId!);

            await baseHubService.AddToGroupAsync(result.Data!.GameId.ToString(),
                receiverConnectionInformation.Data.UserConnectionDTO.ConnectionId!);

            //Send a message to the inviter that the invitation has been accepted and the game has been created with the gameId and the connection information of both players
            await baseHubService.SendAcceptedInviteAsync(
                inviterConnectionInformation.Data.UserConnectionDTO.ConnectionId!,
                inviterConnectionInformation.Data.UserConnectionDTO,
                acceptInvitationRequest.inviterUserGuid,
                receiverConnectionInformation.Data.UserConnectionDTO,
                acceptInvitationRequest.receiverUserGuid,
                result.Data.GameId);


            //Send a message to the receiver that the invitation has been accepted and the game has been created with the gameId and the connection information of both players
            return ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                InvitationResponseDTO,
                ChessGameResponseMessage.SuccessInvitation,
                HttpStatusCode.Created);

        }

        public Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
        {
            throw new NotImplementedException();
        }

        public async Task<PipeLineResponse<SendInvitationsResponseDTO, ChessGameResponseMessage>> SendInviteAsync(SendInvitationRequestDTO connectionRequestDTO)
        {
            await baseHubService.SendInviteAsync(connectionRequestDTO);
            return new PipeLineResponse<SendInvitationsResponseDTO, ChessGameResponseMessage>()
            {
                Response = ResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    ChessGameResponseMessage.SuccessInvitation, HttpStatusCode.Created, []),

            };
        }
    }
}
