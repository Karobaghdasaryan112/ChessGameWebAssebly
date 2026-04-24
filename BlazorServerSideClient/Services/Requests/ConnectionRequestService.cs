using BlazorServerSideClient.Contracts.Requests;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;

namespace BlazorServerSideClient.Services.Requests
{
    public class ConnectionRequestService(SignalRService signalRService) : IConnectionReqeustService
    {
        public async Task<PipeLineResponse<GetUserConnectionResponseDTO>> GetUserConnection(
            PipeLineRequest<GetUserConnectionRequestDTO> getUserConnectionRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.InvokeAsync<PipeLineResponse<GetUserConnectionResponseDTO>>
                ("GetUserConnection", getUserConnectionRequestDTO);
        }

        public async Task<PipeLineResponse<AddUserConnectionResponseDTO>> AddConnectionAsync(
            PipeLineRequest<AddUserConnectionRequestDTO> addUserConnectionRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.InvokeAsync<PipeLineResponse<AddUserConnectionResponseDTO>>
                ("AddConnectionAsync", addUserConnectionRequestDTO);
        }

        public async Task<PipeLineResponse<RemoveUserConnectionResponseDTO>> RemoveConnectionAsync(
            PipeLineResponse<RemoveUserConnectionRequestDTO> removeUserConnectionRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.InvokeAsync<PipeLineResponse<RemoveUserConnectionResponseDTO>>
                ("RemoveConnectionAsync", removeUserConnectionRequestDTO);
        }
    }
}
