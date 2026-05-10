using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;

public class BoardStateSenderResponseDTO : IResponseDTO
{
    public BoardStateResponseDTO  BoardStateResponse { get; set; }
}