using BlazorServerSideClient.Contracts.Requests;
using BlazorServerSideClient.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;

namespace BlazorServerSideClient.Services.Requests
{
    public class ConnectionRequestService(SignalRService signalRService,JSRunetimeService jsRuneTimeService) : IConnectionReqeustService
    {
        public async Task<PipeLineResponse<GetUserConnectionResponseDTO>> GetUserConnection(
            PipeLineRequest<GetUserConnectionRequestDTO> getUserConnectionRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.SafeInvokeAsync<GetUserConnectionRequestDTO, GetUserConnectionResponseDTO>
                       ("GetUserConnection", getUserConnectionRequestDTO.Request, jsRuneTimeService) ??
                   PipeLineResponse<GetUserConnectionResponseDTO>.Emoty;
        }

        public async Task<PipeLineResponse<AddUserConnectionResponseDTO>> AddConnectionAsync(
            PipeLineRequest<AddUserConnectionRequestDTO> addUserConnectionRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.SafeInvokeAsync<AddUserConnectionRequestDTO,AddUserConnectionResponseDTO>
                ("AddConnectionAsync", addUserConnectionRequestDTO.Request,jsRuneTimeService) ?? PipeLineResponse<AddUserConnectionResponseDTO>.Emoty;
        }

        public async Task<PipeLineResponse<RemoveUserConnectionResponseDTO>?> RemoveConnectionAsync(
            PipeLineRequest<RemoveUserConnectionRequestDTO> removeUserConnectionRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.SafeInvokeAsync<RemoveUserConnectionRequestDTO,RemoveUserConnectionResponseDTO>(
                "RemoveConnectionAsync", removeUserConnectionRequestDTO.Request,jsRuneTimeService);
           
        }
    }
}
