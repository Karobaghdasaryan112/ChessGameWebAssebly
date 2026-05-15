using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Enums.Scores;
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
    public interface IFigure : ICloneable
    {
        double GetPositionalScore(Position position, GamePhase phase, bool isWhite)
        {
            var table = phase switch
            {
                GamePhase.Midgame => MidGameTable,
                GamePhase.StartGame => StartGameTable,
                _ => EndGameTable
            };

            int row = isWhite ? (int)position.VerticalOrientation : 7 - (int)position.VerticalOrientation;
            if (row < 0 || row > 7)
                throw new ArgumentOutOfRangeException(nameof(position.VerticalOrientation), "VerticalOrientation must be between 0 and 7.");
            return table[row][(int)position.HorizontalOrientation];
        }
        bool IsMoves { get; set; }
        double[][] MidGameTable { get; }
        double[][] StartGameTable { get; }
        double[][] EndGameTable { get; }
        string FigurePath => $"{FigureType}{FigureColor}.png";
        FigureType FigureType { get; }
        FigureColors FigureColor { get; set; }
        MovableAndCutablePositions GetMovableAndCuttableBlocks(Position position, Board board, Block? kingBlockForCheckCondition = null);
        string GetFenChar();
    }
}
