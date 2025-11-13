using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Collections.Concurrent;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IConnectionService<THub> where THub : Microsoft.AspNetCore.SignalR.Hub
    {
        ConcurrentDictionary<Guid, UserConnectionDTO> CurrentConnectionState { get; }
        ConnectionResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage> GetUserConnection(ConnectionRequestDTO<GetUserConnectionRequestDTO> getUserConnectionRequestDTO);
        Task<ConnectionResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(AddUserConnectionRequestDTO addUserConnectionRequestDTO);
        Task<ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsUserGuidAsync(ConnectionRequestDTO<RemoveUserConnectionRequestDTO> removeUserConnectionRequestDTO);
        Task<ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsConnectionIdAsync(RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO);
    }
}
