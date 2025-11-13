using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;

namespace BlazorServerSideClient.Contracts.Handlers
{
    public interface IConnectionHandlerService
    {
        Action<KeyValuePair<Guid, UserConnectionDTO>>? OnlinePlayersUpdated { get; set; }
        void ReceiveUpdatedUsers(KeyValuePair<Guid, UserConnectionDTO> userConnection);
    }
}
