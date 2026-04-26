using SharedResources.Contracts;

namespace SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;

public class SameFigureResposneDTO : IResponseDTO
{
    public bool IsSameFigure { get; set; }
}