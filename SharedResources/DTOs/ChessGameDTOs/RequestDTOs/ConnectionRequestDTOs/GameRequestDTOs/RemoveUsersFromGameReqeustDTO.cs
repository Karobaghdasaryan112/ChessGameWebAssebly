using SharedResources.Contracts;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

public class RemoveUsersFromGameReqeustDTO : RequestDTO
{
    public Guid CurerntPlayerGuid { get; set; }
    public Guid GameId { get; set; }
    public bool IsLeaveWebSite { get; set; }
}