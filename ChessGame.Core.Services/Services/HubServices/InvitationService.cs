using ChessGame.Core.Services.Contracts.Hub;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class InvitationService<THub> : IInvitationService<THub> where THub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly BaseHubService<THub> _baseHubService;
        private readonly IConnectionService<THub> _connectionService;
        public InvitationService(IConnectionService<THub> connectionService, BaseHubService<THub> baseHubService)
        {
            _connectionService = connectionService;
            _baseHubService = baseHubService;
        }
        public async Task<IResponseTypes<InvitationResponseDTO, ChessGameResponseMessage>>
            AcceptInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
        {

            var playersInformation = new KeyValuePair<Guid, Guid>(inviterUserGuid, receiverUserGuid);
            var gameGuid = Guid.NewGuid();


            var inviterConnectionInformation = _connectionService.GetUserConnection(inviterUserGuid);
            if (!inviterConnectionInformation.IsSuccess)
                return ChessGameResponse<InvitationResponseDTO>.
                     CreateErrorResponse(
                         inviterConnectionInformation.message,
                         inviterConnectionInformation.StatusCode,
                         inviterConnectionInformation.Errors);

            var receiverConnectionInformation = _connectionService.GetUserConnection(receiverUserGuid);
            if (!receiverConnectionInformation.IsSuccess)
                return ChessGameResponse<InvitationResponseDTO>.
                     CreateErrorResponse(
                         receiverConnectionInformation.message,
                         receiverConnectionInformation.StatusCode,
                         receiverConnectionInformation.Errors);

            var gameGuidAsString = gameGuid.ToString();

            await _baseHubService.AddToGroupAsync(gameGuidAsString, inviterConnectionInformation.Data?.ConnectionId!);
            await _baseHubService.AddToGroupAsync(gameGuidAsString, receiverConnectionInformation.Data?.ConnectionId!);

            await _baseHubService.SendAcceptedInviteAsync(inviterConnectionInformation.Data?.ConnectionId!, gameGuid);

            var InvitationResponseDTO = new InvitationResponseDTO()
            {
                GameId = gameGuid,
                PlayerOne_UserConnectionResponseDTO = inviterConnectionInformation.Data,
                PlayerTwo_UserConnectionResponseDTO = receiverConnectionInformation.Data
            };

            return ChessGameResponse<InvitationResponseDTO>.
                     CreateSuccessResponse(
                         InvitationResponseDTO,
                         ChessGameResponseMessage.SuccessInvitation,
                         HttpStatusCode.Created);

        }

        public Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
        {
            throw new NotImplementedException();
        }

        public async Task SendInvite(UserConnectionResponseDTO inviterUserConnection, UserConnectionResponseDTO receiverUserConnection)
        {
            await _baseHubService.SendInviteAsync(receiverUserConnection.ConnectionId, inviterUserConnection, receiverUserConnection);
        }
    }
}
