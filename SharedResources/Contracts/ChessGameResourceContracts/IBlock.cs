using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;

namespace SharedResources.Contracts.ChessGameResourceContracts
{
    public interface IBlock
    {
        Position Position { get; set; }
        IFigure Figure { get; set; }
        BlockColor BlockColor { get; set; }
        void ExploreDirection(MovableAndCutablePositions positions, int row, int col, int rowStep, int colStep);
    }
}
