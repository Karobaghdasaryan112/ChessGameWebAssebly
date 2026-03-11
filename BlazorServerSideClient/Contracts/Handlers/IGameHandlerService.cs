using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Handlers
{
    public interface IGameHandlerService
    {
        Task ReseivePlayersAsync(ResponseDTO<ReceivePlayersResponseDTO, ChessGameResponseMessage> connectionResponseDTO);
        Task ReceiveBoardUpdateAsync(BoardStateResponseDTO gameStateconnectionResponseDTO);
        Task NotifyOpponentUserDisconnected(KeyValuePair<Guid, UserConnectionDTO> opponentUserConnection);
    }
}
