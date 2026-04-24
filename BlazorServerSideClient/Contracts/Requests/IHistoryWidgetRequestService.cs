using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;

namespace BlazorServerSideClient.Contracts.Requests
{
    public interface IHistoryWidgetRequestService
    {
        Task<PipeLineResponse<GetAllHistoryWidgetsResponseDTO>>
            GetAllOpponents(PipeLineRequest<GetAllHistoryWidgetRequestDTO> getAllHistoryWidgetsRequestDto);
    }
}
