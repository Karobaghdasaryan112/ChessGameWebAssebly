using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests;
using SharedResources.Validation.ChessGameValidations.ResponseValidations.ConnectionResponses;
using System.Collections.Concurrent;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IConnectionService
    {
        ConcurrentDictionary<Guid, UserConnectionDTO> CurrentConnectionState { get; }
        Task<ResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>> GetUserConnection(ConnectionRequestDTO<GetUserConnectionRequestDTO> getUserConnectionRequestDTO);
        Task<ResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(ConnectionRequestDTO<AddUserConnectionRequestDTO> AddUserConnectionRequestDTO);
        Task<ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsUserGuidAsync(ConnectionRequestDTO<RemoveUserConnectionRequestDTO> removeUserConnectionRequestDTO);
        Task<ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsConnectionIdAsync(RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO);
        Task<ResponseDTO<RemoveUserFromGameResponseDTO, ChessGameResponseMessage>> RemoveUsersFromGameAsync(ConnectionRequestDTO<RemoveUserFromGameRequestDTO> removeUserFromGameRequestDTO);
        Task<ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>> SendBoardStateToClient(ConnectionRequestDTO<BoardStateRequestDTO> boardStateConnectionRequestDTO, string player,
            bool isMyConnection, bool win = false);
    }
}
