using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;

namespace SharedResources.Contracts.ChessGameResourceContracts
{
    public interface IFigure
    {
        string FigurePath => $"{FigureType}{FigureColor}.png";
        FigureType FigureType { get; }
        FigureColors FigureColor {  get; set; }
        MovableAndCutablePositions GetMovableAndCutableBlocks(Position position);
    }
}
