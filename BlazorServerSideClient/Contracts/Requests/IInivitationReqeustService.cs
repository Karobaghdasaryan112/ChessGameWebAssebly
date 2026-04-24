using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Requests
{
    public interface IInivitationReqeustService
    {
        Task<PipeLineResponse<SendInvitationsResponseDTO>> SendInviteAsync(
            PipeLineRequest<SendInvitationRequestDTO> connectionRequestDto);

        //TO DO:
        Task CancelInviteAsync(Guid inviterPlayerGuid, Guid receiverUserGuid);

        Task<PipeLineResponse<AcceptInvitationResponseDTO>> AcceptInviteAsync(
            PipeLineRequest<AcceptInvitationRequestDTO> acceptInvitationRequest);
    }
}
