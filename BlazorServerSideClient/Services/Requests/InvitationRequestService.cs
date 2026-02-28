using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class InvitationRequestService(IServiceScopeFactory serviceScopeFactory) : IInivitationReqeustService
    {
        private readonly SignalRService signalRService = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<SignalRService>();
        public async Task SendInviteAsync(SendInvitationRequestDTO connectionRequestDTO)
        {

            var hubConnection = await signalRService.GetHubConnection();

            await hubConnection.InvokeAsync("SendInviteAsync", connectionRequestDTO);

        }
        public async Task CancelInviteAsync(Guid inviterPlayerGuid, Guid receiverUserGuid)
        {

            var hubConnection = await signalRService.GetHubConnection();

            await hubConnection.InvokeAsync("CancelInviteAsync", inviterPlayerGuid, receiverUserGuid);

        }

        public async Task<ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>> AcceptInviteAsync(AcceptInvitationRequestDTO acceptInvitationRequest)
        {
            var hubConnection = await signalRService.GetHubConnection();
            return await hubConnection.
                InvokeAsync<
                    ResponseDTO<
                        AcceptInvitationResponseDTO,
                        ChessGameResponseMessage>>
                        ("AcceptInviteAsync", acceptInvitationRequest);
        }
    }
}