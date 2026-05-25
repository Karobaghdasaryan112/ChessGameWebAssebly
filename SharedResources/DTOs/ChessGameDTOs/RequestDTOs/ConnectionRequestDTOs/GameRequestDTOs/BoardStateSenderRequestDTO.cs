using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

public class BoardStateSenderRequestDTO : RequestDTO
{
    public BoardStateRequestDTO BoardStateRequestDTO { get; set; }
    public string Player { get; set; }
    public bool IsMyConnection { get; set; }
    public bool? Win { get; set; }
}