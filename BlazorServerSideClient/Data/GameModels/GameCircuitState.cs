using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;

namespace BlazorServerSideClient.Data.GameModels;

public sealed class GameCircuitState
{
    public Guid? UserId { get; set; }

    public Guid? GameId { get; set; }

    public UserConnectionDTO? UserConnectionDto { get; set; }

    public bool IsJoinedToGame =>
        UserId.HasValue &&
        GameId.HasValue &&
        UserConnectionDto is not null;

    public void Clear()
    {
        UserId = null;
        GameId = null;
        UserConnectionDto = null;
    }
}