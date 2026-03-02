using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class InvitationRequestService(SignalRService signalRService, ILogger<IInivitationReqeustService> logger, JSRunetimeService jSRunetimeService) : IInivitationReqeustService
    {

        public async Task SendInviteAsync(SendInvitationRequestDTO connectionRequestDTO)
            => await jSRunetimeService.SendAsync<SendInvitationRequestDTO, Task>(
                "SendInviteAsync",
                connectionRequestDTO);

        public async Task CancelInviteAsync(Guid inviterPlayerGuid, Guid receiverUserGuid)
            => await jSRunetimeService.SendAsync<(Guid,Guid),Task>(
                "CancelInviteAsync",
                (inviterPlayerGuid,receiverUserGuid));

        public Task<ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>> AcceptInviteAsync(AcceptInvitationRequestDTO acceptInvitationRequest)
            => jSRunetimeService.
            SendAsync<
                AcceptInvitationRequestDTO,
                ResponseDTO<
                    AcceptInvitationResponseDTO,
                    ChessGameResponseMessage>>(
                "AcceptInviteAsync",
                acceptInvitationRequest);
    }
}