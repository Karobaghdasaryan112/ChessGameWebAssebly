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
    public class HistoryWidgetRequestService(IServiceScopeFactory serviceScopeFactory) : IHistoryWidgetRequestService
    {
        private readonly SignalRService signalRService = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<SignalRService>();

        public async Task<ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>
            GetAllOpponents(GetAllHistoryWidgetRequestDTO getAllHistoryWidgetsRequestDTO)
        {
            var hubConnection = await signalRService.GetHubConnection();
            return await hubConnection.InvokeAsync<
                ResponseDTO<
                    GetAllHistoryWidgetsResponseDTO,
                    ChessGameResponseMessage>>("GetAllOpponents", getAllHistoryWidgetsRequestDTO);

        }
    }
}
