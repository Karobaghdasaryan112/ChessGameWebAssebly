using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace BlazorServerSideClient.Contracts.Handlers
{
    public interface IConnectionHandlerService
    {
        Action<KeyValuePair<Guid, UserConnectionDTO>>? OnlinePlayersUpdated { get; set; }
        Task ReceiveUpdatedUsers(Guid userGuid, UserConnectionDTO userConnection);
        void DisconnectedNotification(KeyValuePair<Guid, UserConnectionDTO> opponentUserConnection);
    }
}
