using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Handlers
{
    public interface IGameHandlerService
    {
        public static event Action<FigureColors, TimeSpan, TimeSpan>? OnTickReceived;

        Task ReceiveTick(FigureColors figureColor, TimeSpan whiteSpan, TimeSpan blackSpan);
        Task ReseivePlayersAsync(ResponseDTO<ReceivePlayersResponseDTO, ChessGameResponseMessage> connectionResponseDTO);

        Task ReceiveBoardUpdateAsync(
            ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage> gameStateconnectionResponseDto);
        Task NotifyOpponentUserDisconnected(KeyValuePair<Guid, UserConnectionDTO> opponentUserConnection);
        Task NotifyOpponentLeftWinAsync(string leavingPlayerName);
        Task RedirectToDashboardAsync();
    }
}
