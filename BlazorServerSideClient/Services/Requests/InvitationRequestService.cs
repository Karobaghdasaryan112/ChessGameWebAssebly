using BlazorServerSideClient.Contracts.Requests;
using BlazorServerSideClient.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;

namespace BlazorServerSideClient.Services.Requests
{
    public class InvitationRequestService(
        SignalRService signalRService,
        JSRunetimeService jsRunetimeService) : IInivitationReqeustService
    {
        public async Task<PipeLineResponse<SendInvitationsResponseDTO>> SendInviteAsync(
            PipeLineRequest<SendInvitationRequestDTO> connectionRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();
            return await hubConnection.SafeInvokeAsync<SendInvitationRequestDTO, SendInvitationsResponseDTO>(
                "SendInviteAsync",
                connectionRequestDto.Request, jsRunetimeService) ?? PipeLineResponse<SendInvitationsResponseDTO>.Emoty;
        }

        public async Task CancelInviteAsync(Guid inviterPlayerGuid, Guid receiverUserGuid)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();
            //TO DO: Implement SafeInvoke Async and (Request - Response)
            
            await hubConnection.InvokeAsync("CancelInviteAsync", inviterPlayerGuid, receiverUserGuid);
        }

        public async Task<PipeLineResponse<AcceptInvitationResponseDTO>> AcceptInviteAsync(
            PipeLineRequest<AcceptInvitationRequestDTO> acceptInvitationRequest)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.SafeInvokeAsync<AcceptInvitationRequestDTO, AcceptInvitationResponseDTO>(
                       "AcceptInviteAsync", acceptInvitationRequest.Request, jsRunetimeService) ??
                   PipeLineResponse<AcceptInvitationResponseDTO>.Emoty;
        }
    }
}