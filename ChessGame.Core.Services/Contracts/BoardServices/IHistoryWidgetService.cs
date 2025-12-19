using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Contracts.BoardServices
{
    public interface IHistoryWidgetService
    {
        Task<ResponseDTO<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>>
        GetGamesByCurrentAndOpponentIdsPagination(
            GetGamesByCurrentAndOpponentIdsPaginationRequestDTO requestDto);

        Task<ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>
                   GetAllOpponents(GetAllHistoryWidgetRequestDTO getAllHistoryReqeustDTO);
    }
}
