using Microsoft.AspNetCore.SignalR;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Collections.Concurrent;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IConnectionService<THub> where THub : Microsoft.AspNetCore.SignalR.Hub
    {
        ConcurrentDictionary<Guid, UserConnectionResponseDTO> CurrentConnectionState { get; }
        IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage> GetUserConnection(Guid userGuid);
        Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(Guid userGuid, UserConnectionResponseDTO connection);
        Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsync(Guid userGuid);
        Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsync(string connectionId);
    }
}
