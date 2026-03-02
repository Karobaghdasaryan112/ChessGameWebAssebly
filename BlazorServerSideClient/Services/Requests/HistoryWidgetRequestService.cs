using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class HistoryWidgetRequestService(SignalRService signalRService, JSRunetimeService jSRunetimeService) : IHistoryWidgetRequestService
    {
        public Task<ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>
            GetAllOpponents(GetAllHistoryWidgetRequestDTO getAllHistoryWidgetsRequestDTO)
        => jSRunetimeService.
            SendAsync<
            GetAllHistoryWidgetRequestDTO,
            ResponseDTO<
                GetAllHistoryWidgetsResponseDTO,
                ChessGameResponseMessage>>(
                "GetAllOpponents",
                getAllHistoryWidgetsRequestDTO);
    }
}
