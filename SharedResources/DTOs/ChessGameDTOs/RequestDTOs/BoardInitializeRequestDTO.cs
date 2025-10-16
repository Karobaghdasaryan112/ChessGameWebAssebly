using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.ChessGameDTOs.RequestDTOs
{
    public class BoardInitializeRequestDTO : ICheseGameRequestDTO
    {
        public int GameId { get; set; }
        public FigureColors MyFigureColor { get; set; }
    }
}
