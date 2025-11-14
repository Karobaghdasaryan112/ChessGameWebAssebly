using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
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
        public async Task<
            ConnectionResponseDTO<
                AddUserConnectionResponseDTO,
                ChessGameResponseMessage>> GetUserConnection(ConnectionRequestDTO<GetUserConnectionRequestDTO> getUserConnectionRequestDTO)// GetUserConnectionRequestDTO getUserConnectionRequestDTO
        {
            var hubConnection = await _signalRService.GetHubConnection();

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
                ChessGameResponseMessage>> AddConnectionAsync(ConnectionRequestDTO<AddUserConnectionRequestDTO> addUserConnectionRequestDTO)//AddUserConnectionRequestDTO addUserConnectionRequestDTO
        {
            var hubConnection = await _signalRService.GetHubConnection();

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
                ChessGameResponseMessage>> RemoveConnectionAsync(ConnectionRequestDTO<RemoveUserConnectionRequestDTO> removeUserConnectionRequestDTO) //RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO
        {
            var hubConnection = await _signalRService.GetHubConnection();

            return await hubConnection.
                InvokeAsync<
                  ConnectionResponseDTO<
                      RemoveUserConnectionResponseDTO,
                      ChessGameResponseMessage>>
                        ("RemoveConnectionAsync", removeUserConnectionRequestDTO.Data.UserGuid);
        }
    }
}
