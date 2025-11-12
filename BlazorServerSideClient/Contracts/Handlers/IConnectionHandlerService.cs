using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;

namespace BlazorServerSideClient.Contracts.Handlers
{
    public interface IConnectionHandlerService
    {
         Action<KeyValuePair<Guid, UserConnectionResponseDTO>>? OnlinePlayersUpdated { get; set; }
        Task ReceiveUpdatedUsers(KeyValuePair<Guid, UserConnectionResponseDTO> userConnection);
    }
}
