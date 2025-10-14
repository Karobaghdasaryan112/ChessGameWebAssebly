using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;

namespace SharedResources.Contracts.ChessGameResourceContracts
{
    public interface IFigure
    {
        FigureType FigureType { get; set; }
        FigureColors FigureColor {  get; set; }
        List<Position> GetMovableAndCutableBlocks(Position position);
    }
}
