using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class InvitationRequestService : IInivitationReqeustService
    {
        private readonly SignalRService _signalRService;
        public InvitationRequestService(SignalRService signalRService)
        {
            _signalRService = signalRService;
        }

        public async Task SendInviteAsync(ConnectionRequestDTO<SendInvitationRequestDTO> connectionRequestDTO)//Guid inviterPlayerId, Guid receiverPlayerId
        {
            var hubConnection = await _signalRService.GetHubConnection();
             await hubConnection.InvokeAsync("SendInviteAsync", connectionRequestDTO);
        }
        public async Task CancelInviteAsync(Guid inviterPlayerGuid, Guid receiverUserGuid)
        {
            var hubConnection = await _signalRService.GetHubConnection();
            //TO:do : Implement CancelInviteAsync in Hub
            await hubConnection.InvokeAsync("CancelInviteAsync", inviterPlayerGuid, receiverUserGuid);
        }

        public async Task<ConnectionResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>> AcceptInviteAsync(ConnectionRequestDTO<AcceptInvitationRequestDTO> acceptInvitationRequest)
        {
            var hubConnection = await _signalRService.GetHubConnection();
            return await hubConnection.
                InvokeAsync<
                    ConnectionResponseDTO<
                        AcceptInvitationResponseDTO,
                        ChessGameResponseMessage>>
                        ("c", acceptInvitationRequest);
        }
    }
}
