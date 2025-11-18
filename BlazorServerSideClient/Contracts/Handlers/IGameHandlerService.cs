using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Handlers
{
    public interface IGameHandlerService
    {
        Task ReseivePlayersAsync(ConnectionResponseDTO<ReceivePlayersResponseDTO, ChessGameResponseMessage> connectionResponseDTO);
    }
}
