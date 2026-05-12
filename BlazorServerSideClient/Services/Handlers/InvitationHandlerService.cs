using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Services;
using Microsoft.AspNetCore.Components;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

public class InvitationHandlerService : IInvitationHandlerService
{
    private readonly JSRunetimeService _jsRuntimeService;
    private readonly NavigationManager _navigationManager;

    public Action<ResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>> OnReceived { get; set; }
    public SendInvitationsResponseDTO? lastInvite { get; set; }

    public InvitationHandlerService(JSRunetimeService jsRuntimeService, NavigationManager navigationManager)
    {
        _jsRuntimeService = jsRuntimeService;
        _navigationManager = navigationManager;
    }

    public async Task ReceiveInvite(
        PlayEvent playEvent,
        UserConnectionDTO inviterUserConnection,
        Guid inviterUserGuid,
        UserConnectionDTO receiverUserConnection,
        Guid receiverUserGuid)
    {
        lastInvite = new SendInvitationsResponseDTO
        {
            InviterUserConnection = inviterUserConnection,
        };

        // Notify the UI first
        OnReceived?.Invoke(new ResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>
        {
            Data = new SendInvitationsResponseDTO
            {
                PlayEvent = playEvent,
                InviterUserConnection = inviterUserConnection,
                InviterUserGuid = inviterUserGuid,
                ReceiverUserConnection = receiverUserConnection,
                ReceiverUserGuid = receiverUserGuid
            },
            Message = ChessGameResponseMessage.SuccessInvitation,
        });

        // Use the safe wrapper you already have in JSRunetimeService
        await _jsRuntimeService.ShowInviteModal(inviterUserConnection.UserName);
    }

    public void InviteAcceptedAsync(
        UserConnectionDTO inviterUserConnection,
        Guid inviterUserGuid,
        UserConnectionDTO receiverUserConnection,
        Guid receiverUserGuid,
        Guid gameGuid)
    {
        // Construct the URL
        var url = $"/game?GameId={gameGuid}" +
                  $"&Player1={Uri.EscapeDataString(inviterUserConnection.UserName)}" +
                  $"&Player2={Uri.EscapeDataString(receiverUserConnection.UserName)}";

        
        _navigationManager.NavigateTo(url);
    }
}