using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IInvitationService<T> where T : Microsoft.AspNetCore.SignalR.Hub
    {

        Task<ConnectionResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>> AcceptInviteAsync(ConnectionRequestDTO<AcceptInvitationRequestDTO> acceptInvitationRequest);
        Task SendInvite(ConnectionRequestDTO<SendInvitationRequestDTO> connectionRequestDTO);
        Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid);
    }
}
