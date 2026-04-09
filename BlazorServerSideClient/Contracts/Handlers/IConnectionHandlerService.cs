using SharedResources.ChessGameResource.Enums.Users;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace BlazorServerSideClient.Contracts.Handlers
{
    public interface IConnectionHandlerService
    {
        Action<OnlinePlayerChangeType, KeyValuePair<Guid, UserConnectionDTO>>? OnlinePlayersUpdated { get; set; }
        void ReceiveUpdatedUsers(KeyValuePair<Guid, UserConnectionDTO> userConnection);
        void DisconnectedNotification(KeyValuePair<Guid, UserConnectionDTO> opponentUserConnection);
    }
}
