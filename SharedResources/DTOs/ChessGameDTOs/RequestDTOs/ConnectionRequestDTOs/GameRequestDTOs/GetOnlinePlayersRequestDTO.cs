using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs
{
    public class GetONlinePlayersRequestDTO : RequestDTO

    {
    public Guid UserGuid { get; set; }
    }
}
