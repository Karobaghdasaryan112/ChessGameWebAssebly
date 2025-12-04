using SharedResources.ChessGameResource.Figures;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace ChessGame.Core.Services.Extentions
{
    public static class FigureExtensions
    {
        public static IFigure FromFenCharToFigure(char fenCharacter)
        {
            return fenCharacter switch
            {
                'K' => new King() { FigureColor = SharedResources.ChessGameResource.Enums.Colors.FigureColors.White },
                'k' => new King() { FigureColor = SharedResources.ChessGameResource.Enums.Colors.FigureColors.Black },
                'Q' => new Queen() { FigureColor = SharedResources.ChessGameResource.Enums.Colors.FigureColors.White },
                'q' => new Queen() { FigureColor = SharedResources.ChessGameResource.Enums.Colors.FigureColors.Black },
                'R' => new Rook() { FigureColor = SharedResources.ChessGameResource.Enums.Colors.FigureColors.White },
                'r' => new Rook() { FigureColor = SharedResources.ChessGameResource.Enums.Colors.FigureColors.Black },
                'B' => new Bishop() { FigureColor = SharedResources.ChessGameResource.Enums.Colors.FigureColors.White },
                'b' => new Bishop() { FigureColor = SharedResources.ChessGameResource.Enums.Colors.FigureColors.Black },
                'N' => new Knight() { FigureColor = SharedResources.ChessGameResource.Enums.Colors.FigureColors.White },
                'n' => new Knight() { FigureColor = SharedResources.ChessGameResource.Enums.Colors.FigureColors.Black },
                'P' => new Pawn() { FigureColor = SharedResources.ChessGameResource.Enums.Colors.FigureColors.White },
                'p' => new Pawn() { FigureColor = SharedResources.ChessGameResource.Enums.Colors.FigureColors.Black },
                _ => throw new ArgumentException("Invalid FEN character for a chess figure."),
            };
        }
    }
}
