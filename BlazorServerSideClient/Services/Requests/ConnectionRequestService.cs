using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class ConnectionRequestService : IConnectionReqeustService
    {
        private readonly SignalRService _signalRService;
        public ConnectionRequestService(SignalRService signalRService)
        {
            _signalRService = signalRService;
        }
        public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> GetUserConnection(Guid userGuid)
        {
            var hubConnection = await _signalRService.GetHubConnection();

            return await hubConnection.
                InvokeAsync<
                    IResponseTypes<
                        UserConnectionResponseDTO,
                        ChessGameResponseMessage>>
                        ("GetUserConnection", userGuid);
        }
        public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(Guid userGuid, UserConnectionResponseDTO userConnection)
        {
            var hubConnection = await _signalRService.GetHubConnection();

            return await hubConnection.
                InvokeAsync<
                    IResponseTypes<
                        UserConnectionResponseDTO,
                        ChessGameResponseMessage>>
                        ("AddConnectionAsync", userGuid, userConnection);
        }
        public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsync(Guid userGuid)
        {
            var hubConnection = await _signalRService.GetHubConnection();

            return await hubConnection.
                InvokeAsync<
                    IResponseTypes<
                        UserConnectionResponseDTO,
                        ChessGameResponseMessage>>
                        ("RemoveConnectionAsync", userGuid);
        }
    }
}
