using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.Requests;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class InvitationService<THub> : IInvitationService<THub> where THub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly BaseHubService<THub> _baseHubService;
        private readonly IConnectionService<THub> _connectionService;
        private readonly IMediator _mediator;
        public InvitationService(IConnectionService<THub> connectionService, BaseHubService<THub> baseHubService, IMediator mediator)
        {
            _connectionService = connectionService;
            _mediator = mediator;
            _baseHubService = baseHubService;
        }
        public async Task<ConnectionResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>>
            AcceptInviteAsync(ConnectionRequestDTO<AcceptInvitationRequestDTO> acceptInvitationRequest)
        {

            var playersInformation = new KeyValuePair<Guid, Guid>(acceptInvitationRequest.Data.inviterUserGuid, acceptInvitationRequest.Data.receiverUserGuid);
            var gameGuid = Guid.NewGuid();


            var inviterConnectionInformation = _connectionService.GetUserConnection(new ConnectionRequestDTO<GetUserConnectionRequestDTO>() { Data = new GetUserConnectionRequestDTO() { UserGuid = acceptInvitationRequest.Data.inviterUserGuid } });
            if (!inviterConnectionInformation.IsSuccess)
                return ConnectionResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>.
                     CreateErrorResponse(
                         default!,
                         inviterConnectionInformation.Message,
                         inviterConnectionInformation.HttpStatusCode,
                         inviterConnectionInformation.Errors);

            var receiverConnectionInformation = _connectionService.GetUserConnection(new ConnectionRequestDTO<GetUserConnectionRequestDTO>() { Data = new GetUserConnectionRequestDTO() { UserGuid = acceptInvitationRequest.Data.receiverUserGuid } });
            if (!receiverConnectionInformation.IsSuccess)
                return ConnectionResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>.
                     CreateErrorResponse(
                         default!,
                         receiverConnectionInformation.Message,
                         receiverConnectionInformation.HttpStatusCode,
                         receiverConnectionInformation.Errors);

            var gameGuidAsString = gameGuid.ToString();


            var command = new BoardInitializeCommand< IRequestTypes<BoardInitializeRequestDTO>, IResponseTypes<BoardInitializeResponseDTO, ChessGameResponseMessage> >

            (new ChessGameRequest<BoardInitializeRequestDTO>()
            {
                requestType = new BoardInitializeRequestDTO()
                {
                    Player1Id = acceptInvitationRequest.Data.inviterUserGuid,
                    Player2Id = acceptInvitationRequest.Data.receiverUserGuid
                }
            });

                var result = await _mediator.Send(command);

                await _baseHubService.AddToGroupAsync(result.Data!.GameId.ToString(), inviterConnectionInformation.Data.UserConnectionDTO.ConnectionId!);
                await _baseHubService.AddToGroupAsync(result.Data!.GameId.ToString(), receiverConnectionInformation.Data.UserConnectionDTO.ConnectionId!);



            await _baseHubService.SendAcceptedInviteAsync(
                inviterConnectionInformation.Data.UserConnectionDTO.ConnectionId!,
                inviterConnectionInformation.Data.UserConnectionDTO,
                acceptInvitationRequest.Data.inviterUserGuid,
                receiverConnectionInformation.Data.UserConnectionDTO,
                acceptInvitationRequest.Data.receiverUserGuid,
                result.Data.GameId);

            inviterConnectionInformation.Data.UserConnectionDTO.GameId = result.Data.GameId;
            receiverConnectionInformation.Data.UserConnectionDTO.GameId = result.Data.GameId;


            var InvitationResponseDTO = new AcceptInvitationResponseDTO()
            {
                GameId = result.Data.GameId,
                PlayerOne_UserConnectionResponseDTO = inviterConnectionInformation.Data.UserConnectionDTO,
                PlayerTwo_UserConnectionResponseDTO = receiverConnectionInformation.Data.UserConnectionDTO
            };

            ConnetionService<THub>._connections[acceptInvitationRequest.Data.receiverUserGuid].GameId = result.Data.GameId;
            ConnetionService<THub>._connections[acceptInvitationRequest.Data.receiverUserGuid].Gameinfo =
                new Gameinfo()
                {
                    Players = new KeyValuePair<Guid, Guid>(
                        acceptInvitationRequest.Data.receiverUserGuid,
                        acceptInvitationRequest.Data.inviterUserGuid)
                };

            ConnetionService<THub>._connections[acceptInvitationRequest.Data.inviterUserGuid].GameId = result.Data.GameId;
            ConnetionService<THub>._connections[acceptInvitationRequest.Data.inviterUserGuid].Gameinfo =
                new Gameinfo()
                {
                    Players = new KeyValuePair<Guid, Guid>(
                        acceptInvitationRequest.Data.receiverUserGuid,
                        acceptInvitationRequest.Data.inviterUserGuid)
                };

            return ConnectionResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>.
                     CreateSuccessResponse(
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

            await _baseHubService.SendInviteAsync(connectionRequestDTO);
        }
    }
}
