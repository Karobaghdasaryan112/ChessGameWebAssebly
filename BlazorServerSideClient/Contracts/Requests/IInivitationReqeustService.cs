using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Requests
{
    public interface IInivitationReqeustService
    {
        Task<ConnectionResponseDTO<SendInvitationRequestDTO, ChessGameResponseMessage>> SendInviteAsync(ConnectionRequestDTO<SendInvitationRequestDTO> connectionResponseDTO);
        Task CancelInviteAsync(Guid inviterPlayerGuid, Guid receiverUserGuid);
        Task<ConnectionResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>> AcceptInviteAsync(ConnectionRequestDTO<AcceptInvitationRequestDTO> acceptInvitationRequest);
    }
}
