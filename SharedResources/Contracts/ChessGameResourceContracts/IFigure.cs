using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Figures;
using SharedResources.ChessGameResource.Models;
using System.Text.Json.Serialization;

namespace SharedResources.Contracts.ChessGameResourceContracts
{
    [JsonDerivedType(typeof(King), "king")]
    [JsonDerivedType(typeof(Queen), "queen")]
    [JsonDerivedType(typeof(Rook), "rook")]
    [JsonDerivedType(typeof(Bishop), "bishop")]
    [JsonDerivedType(typeof(Knight), "knight")]
    [JsonDerivedType(typeof(Pawn), "pawn")]
    public interface IFigure
    {
        string FigurePath => $"{FigureType}{FigureColor}.png";
        FigureType FigureType { get; }
        FigureColors FigureColor {  get; set; }
        MovableAndCutablePositions GetMovableAndCutableBlocks(Position position, Board board, Block kingBlockForCheckCondition = default);
    }
}
