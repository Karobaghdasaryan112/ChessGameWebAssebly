using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Services;
using Microsoft.AspNetCore.Components;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

public class InvitationHandlerService : IInvitationHandlerService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public Action<ResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>> OnReceived { get; set; }
    public SendInvitationsResponseDTO? lastInvite { get; set; }

    public InvitationHandlerService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void ReceiveInvite(
        UserConnectionDTO inviterUserConnection,
        Guid inviterUserGuid,
        UserConnectionDTO receiverUserConnection,
        Guid receiverUserGuid)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var jsRuntime = scope.ServiceProvider.GetRequiredService<JSRunetimeService>();
        var nav = scope.ServiceProvider.GetRequiredService<NavigationManager>();

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

        jsRuntime.ShowInviteModal(15, inviterUserConnection.UserName);
    }

    public void InviteAcceptedAsync(
        UserConnectionDTO inviterUserConnection,
        Guid inviterUserGuid,
        UserConnectionDTO receiverUserConnection,
        Guid receiverUserGuid,
        Guid gameGuid)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var nav = scope.ServiceProvider.GetRequiredService<NavigationManager>();
        nav.NavigateTo($"/game?GameId={gameGuid}&Player1={inviterUserConnection.UserName}&Player2={receiverUserConnection.UserName}", true);
    }

  
}