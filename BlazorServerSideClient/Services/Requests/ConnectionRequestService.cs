using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class ConnectionRequestService(SignalRService signalRService) : IConnectionReqeustService
    {
        public async Task<
            ConnectionResponseDTO<
                AddUserConnectionResponseDTO,
                ChessGameResponseMessage>> GetUserConnection(ConnectionRequestDTO<GetUserConnectionRequestDTO> getUserConnectionRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnection();

            return await hubConnection.
                InvokeAsync<
                    ConnectionResponseDTO<
                        AddUserConnectionResponseDTO,
                        ChessGameResponseMessage>>
                        ("GetUserConnection", getUserConnectionRequestDTO);
        }
        public async Task<
            ConnectionResponseDTO<
                AddUserConnectionResponseDTO,
                ChessGameResponseMessage>> AddConnectionAsync(ConnectionRequestDTO<AddUserConnectionRequestDTO> addUserConnectionRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnection();

            return await hubConnection.
                InvokeAsync<
                    ConnectionResponseDTO<
                        AddUserConnectionResponseDTO,
                        ChessGameResponseMessage>>
                        ("AddConnectionAsync", addUserConnectionRequestDTO);
        }
        public async Task<
            ConnectionResponseDTO<
                RemoveUserConnectionResponseDTO,
                ChessGameResponseMessage>> RemoveConnectionAsync(ConnectionRequestDTO<RemoveUserConnectionRequestDTO> removeUserConnectionRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnection();

            return await hubConnection.
                InvokeAsync<
                  ConnectionResponseDTO<
                      RemoveUserConnectionResponseDTO,
                      ChessGameResponseMessage>>
                        ("RemoveConnectionAsync", removeUserConnectionRequestDTO.Data.UserGuid);
        }
    }
}
