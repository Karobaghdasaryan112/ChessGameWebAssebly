using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IInvitationService
    {

        Task<ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>>
                    AcceptInviteAsync(AcceptInvitationRequestDTO acceptInvitationRequest);
        Task SendInviteAsync(ConnectionRequestDTO<SendInvitationRequestDTO> connectionRequestDTO);
        Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid);
    }
}
