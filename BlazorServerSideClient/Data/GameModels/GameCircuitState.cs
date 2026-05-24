using Microsoft.AspNetCore.Components.Server.Circuits;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace BlazorServerSideClient.Data.GameModels;

public class GameCircuitState : CircuitHandler
{
    public Guid? UserId { get; set; }
    public Guid? GameId { get; set; }
    
    public UserConnectionDTO UserConnectionDto { get; set; }
    // public bool IsInGame => UserId is not new Guid(  00000000-0000-0000-0000-000000000000 */) && GameId is not null;
}