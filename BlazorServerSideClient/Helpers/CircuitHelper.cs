using Microsoft.AspNetCore.Components.Server.Circuits;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace BlazorServerSideClient.Helpers;

public static class CircuitHelper
{
    public static Dictionary<Circuit, UserConnectionDTO> CircuitAndUserConnections =
        new Dictionary<Circuit, UserConnectionDTO>();

    public static async Task<bool> TryAddToCircuit(Circuit circuit, UserConnectionDTO userConnection)
        => CircuitAndUserConnections.TryAdd(circuit, userConnection);

    public static async Task<bool> TryRemoveFromCircuit(Circuit circuit, UserConnectionDTO userConnection)
        => CircuitAndUserConnections.Remove(circuit);

    public static bool TryGetValue(Circuit circuit, out UserConnectionDTO connection)
    {
         CircuitAndUserConnections.TryGetValue(circuit, out var userConnection);
         connection = userConnection;
         return userConnection != null;
    }

    public static async Task<string> TryGetHubConnection(Circuit circuit)
        => CircuitAndUserConnections.TryGetValue(circuit, out var userConnection)
            ? userConnection.ConnectionId
            : string.Empty;
}