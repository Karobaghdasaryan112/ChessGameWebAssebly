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
using SharedResources.PipeLine.PipeLineContext;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IConnectionService
    {
        ConcurrentDictionary<Guid, UserConnectionDTO> CurrentConnectionState { get; }

        Task<PipeLineResponse<GetUserConnectionResponseDTO>> GetUserConnection(
            GetUserConnectionRequestDTO getUserConnectionRequestDTO);

        Task<PipeLineResponse<AddUserConnectionResponseDTO>> AddConnectionAsync(
            AddUserConnectionRequestDTO AddUserConnectionRequestDTO);

        Task<PipeLineResponse<RemoveUserConnectionResponseDTO>>
            RemoveConnectionAsUserGuidAsync(
                RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO);

        Task<PipeLineResponse<RemoveUserConnectionResponseDTO>> RemoveConnectionAsConnectionIdAsync(
            RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO);

        Task<PipeLineResponse<RemoveUserFromGameResponseDTO>> RemoveUsersFromGameAsync(
            RemoveUserFromGameRequestDTO removeUserFromGameRequestDTO);

        Task<PipeLineResponse<BoardStateSenderResponseDTO>>
            SendBoardStateToClient(BoardStateSenderRequestDTO sendBoardStateReqeust);

        Task<PipeLineResponse<DisconnectedUserNotificationResponseDTO>> NotifyDisconnectedUser(
            DisconnectedUserNotificationRequestDTO disconnectedUserNotificationRequestDTO);
    }
}