using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;

namespace BlazorServerSideClient.Contracts.Requests
{
    public interface IConnectionReqeustService
    {
        Task<PipeLineResponse<GetUserConnectionResponseDTO>>
            GetUserConnection(PipeLineRequest<GetUserConnectionRequestDTO> getUserConnectionRequestDTO);

        Task<PipeLineResponse<AddUserConnectionResponseDTO>>
            AddConnectionAsync(PipeLineRequest<AddUserConnectionRequestDTO> addUserConnectionRequestDTO);

        Task<PipeLineResponse<RemoveUserConnectionResponseDTO>>
            RemoveConnectionAsync(PipeLineResponse<RemoveUserConnectionRequestDTO> removeUserConnectionRequestDTO);
    }
}
