using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Contracts.BoardServices
{
    public interface IHistoryWidgetService
    {
        Task<IResponseTypes<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>>
            GetGamesByCurrentAndOpponentIdsPagination(
                IRequestTypes<GetGamesByCurrentAndOpponentIdsPaginationRequestDTO> RequestDto);

        Task<ConnectionResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>
            GetAllOpponents(ConnectionRequestDTO<GetAllHistoryWidgetRequestDTO> getAllHistoryReqeustDTO);
    }
}
