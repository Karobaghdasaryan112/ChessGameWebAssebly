using BlazorServerSideClient.Contracts.Requests;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;

namespace BlazorServerSideClient.Services.Requests
{
    public class HistoryWidgetRequestService(SignalRService signalRService) : IHistoryWidgetRequestService
    {
        public async Task<PipeLineResponse<GetAllHistoryWidgetsResponseDTO>>
            GetAllOpponents(PipeLineRequest<GetAllHistoryWidgetRequestDTO> getAllHistoryWidgetsRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.InvokeAsync<PipeLineResponse<GetAllHistoryWidgetsResponseDTO>>("GetAllOpponents",
                getAllHistoryWidgetsRequestDto);

        }
    }
}
