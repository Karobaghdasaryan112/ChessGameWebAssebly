using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class InvitationRequestService(
        SignalRService signalRService) : IInivitationReqeustService
    {
        public async Task SendInviteAsync(SendInvitationRequestDTO connectionRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnection();

            await hubConnection.InvokeAsync("SendInviteAsync", connectionRequestDto);
        }

        public async Task CancelInviteAsync(Guid inviterPlayerGuid, Guid receiverUserGuid)
        {
            var hubConnection = await signalRService.GetHubConnection();

            await hubConnection.InvokeAsync("CancelInviteAsync", inviterPlayerGuid, receiverUserGuid);
        }

        public async Task<ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>> AcceptInviteAsync(
            AcceptInvitationRequestDTO acceptInvitationRequest)
        {
            var hubConnection = await signalRService.GetHubConnection();
            return await hubConnection.InvokeAsync<
                    ResponseDTO<
                        AcceptInvitationResponseDTO,
                        ChessGameResponseMessage>>
                ("AcceptInviteAsync", acceptInvitationRequest);
        }
    }
}