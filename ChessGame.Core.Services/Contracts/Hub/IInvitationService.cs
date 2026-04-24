using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IInvitationService
    {

        Task<PipeLineResponse<AcceptInvitationResponseDTO>> AcceptInviteAsync(
            AcceptInvitationRequestDTO acceptInvitationRequest);

        Task<PipeLineResponse<SendInvitationsResponseDTO>> SendInviteAsync(
            SendInvitationRequestDTO connectionRequestDto);
        Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid);
    }
}
