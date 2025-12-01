using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Handlers
{
    public interface IGameHandlerService
    {
        Task ReseivePlayersAsync(ConnectionResponseDTO<ReceivePlayersResponseDTO, ChessGameResponseMessage> connectionResponseDTO);
        Task ReceiveBoardUpdateAsync(ConnectionResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage> gameStateconnectionResponseDTO);
    }
}
