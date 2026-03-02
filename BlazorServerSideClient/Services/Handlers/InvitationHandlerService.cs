using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

public class InvitationHandlerService : IInvitationHandlerService
{
    public Action<ResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>> OnReceived { get; set; }
    public SendInvitationsResponseDTO? lastInvite { get; set; }
    private JSRunetimeService _jsRuntime { get; set; }
    private NavigationManager _nav { get; set; }
    public InvitationHandlerService(NavigationManager nav,JSRunetimeService jsRuntime)
    {
        this._jsRuntime = jsRuntime;    
        this._nav = nav;
    }

    public void ReceiveInvite(
        UserConnectionDTO inviterUserConnection,
        Guid inviterUserGuid,
        UserConnectionDTO receiverUserConnection,
        Guid receiverUserGuid)
    {
        lastInvite = new SendInvitationsResponseDTO
        {
            InviterUserConnection = inviterUserConnection
        };

        OnReceived?.Invoke(new ResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>
        {
            Data = new SendInvitationsResponseDTO
            {
                InviterUserConnection = inviterUserConnection,
                InviterUserGuid = inviterUserGuid,
                ReceiverUserConnection = receiverUserConnection,
                ReceiverUserGuid = receiverUserGuid
            },
            Message = ChessGameResponseMessage.SuccessInvitation
        });

        _jsRuntime.ShowInviteModal(15, inviterUserConnection.UserName);
    }

    public void InviteAcceptedAsync(
        UserConnectionDTO inviterUserConnection,
        Guid inviterUserGuid,
        UserConnectionDTO receiverUserConnection,
        Guid receiverUserGuid,
        Guid gameGuid)
    {
        _nav.NavigateTo($"/game?GameId={gameGuid}&Player1={inviterUserConnection.UserName}&Player2={receiverUserConnection.UserName}", true);
    }

  
}