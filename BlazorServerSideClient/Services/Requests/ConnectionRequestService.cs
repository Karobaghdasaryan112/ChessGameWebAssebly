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
            ResponseDTO<
                AddUserConnectionResponseDTO,
                ChessGameResponseMessage>> GetUserConnection(GetUserConnectionRequestDTO getUserConnectionRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnection();

            return await hubConnection.
                InvokeAsync<
                    ResponseDTO<
                        AddUserConnectionResponseDTO,
                        ChessGameResponseMessage>>
                        ("GetUserConnection", getUserConnectionRequestDTO);
        }
        public async Task<
            ResponseDTO<
                AddUserConnectionResponseDTO,
                ChessGameResponseMessage>> AddConnectionAsync(AddUserConnectionRequestDTO addUserConnectionRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnection();

            return await hubConnection.
                InvokeAsync<
                    ResponseDTO<
                        AddUserConnectionResponseDTO,
                        ChessGameResponseMessage>>
                        ("AddConnectionAsync", addUserConnectionRequestDTO);
        }
        public async Task<
            ResponseDTO<
                RemoveUserConnectionResponseDTO,
                ChessGameResponseMessage>> RemoveConnectionAsync(RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnection();

            return await hubConnection.
                InvokeAsync<
                  ResponseDTO<
                      RemoveUserConnectionResponseDTO,
                      ChessGameResponseMessage>>
                        ("RemoveConnectionAsync", removeUserConnectionRequestDTO.UserGuid);
        }
    }
}
