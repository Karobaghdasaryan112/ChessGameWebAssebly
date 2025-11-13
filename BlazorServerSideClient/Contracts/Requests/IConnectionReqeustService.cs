using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Requests
{
    public interface IConnectionReqeustService
    {
        Task<ConnectionResponseDTO<AddUserConnectionResponseDTO,ChessGameResponseMessage>> GetUserConnection(ConnectionRequestDTO<GetUserConnectionRequestDTO> getUserConnectionRequestDTO);
        Task<ConnectionResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(ConnectionRequestDTO<AddUserConnectionRequestDTO> addUserConnectionRequestDTO);
        Task<ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsync(ConnectionRequestDTO<RemoveUserConnectionRequestDTO> removeUserConnectionRequestDTO);
    }
}
