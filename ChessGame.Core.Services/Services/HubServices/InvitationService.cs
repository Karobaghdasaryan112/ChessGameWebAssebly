using ChessGame.Core.Services.Contracts.Hub;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
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

            await _baseHubService.AddToGroupAsync(gameGuidAsString, inviterConnectionInformation.Data.UserConnectionDTO.ConnectionId!);
            await _baseHubService.AddToGroupAsync(gameGuidAsString, receiverConnectionInformation.Data.UserConnectionDTO.ConnectionId!);

            //TO DO Save additional information into DataBase
            await _baseHubService.SendAcceptedInviteAsync(
                inviterConnectionInformation.Data.UserConnectionDTO.ConnectionId!,
                inviterConnectionInformation.Data.UserConnectionDTO,
                acceptInvitationRequest.Data.inviterUserGuid,
                receiverConnectionInformation.Data.UserConnectionDTO, 
                acceptInvitationRequest.Data.receiverUserGuid, 
                gameGuid);

            var InvitationResponseDTO = new AcceptInvitationResponseDTO()
            {
                GameId = gameGuid,
                PlayerOne_UserConnectionResponseDTO = inviterConnectionInformation.Data.UserConnectionDTO,
                PlayerTwo_UserConnectionResponseDTO = receiverConnectionInformation.Data.UserConnectionDTO
            };

            ConnetionService<THub>._connections[acceptInvitationRequest.Data.receiverUserGuid].GameId = gameGuid;
            ConnetionService<THub>._connections[acceptInvitationRequest.Data.receiverUserGuid].Gameinfo =
                new Gameinfo()
                { 
                    Players = new KeyValuePair<Guid, Guid>(
                        acceptInvitationRequest.Data.receiverUserGuid, 
                        acceptInvitationRequest.Data.inviterUserGuid) 
                };

            ConnetionService<THub>._connections[acceptInvitationRequest.Data.inviterUserGuid].GameId = gameGuid;
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
