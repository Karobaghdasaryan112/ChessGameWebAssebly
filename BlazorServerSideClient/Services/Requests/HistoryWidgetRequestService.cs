using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class HistoryWidgetRequestService(SignalRService signalRService) : IHistoryWidgetRequestService
    {
        public async Task<ConnectionResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>
            GetAllOpponents(ConnectionRequestDTO<GetAllHistoryWidgetRequestDTO> getAllHistoryWidgetsRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnection();
            return await hubConnection.InvokeAsync<
                ConnectionResponseDTO<
                    GetAllHistoryWidgetsResponseDTO,
                    ChessGameResponseMessage>>("GetAllOpponents", getAllHistoryWidgetsRequestDTO);

        }
    }
}
