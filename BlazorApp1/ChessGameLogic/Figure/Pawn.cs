using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace WebAssemblyChessGame.UI.ChessGameLogic.Figure
{
    public class Pawn : IFigure
    {
        private FigureType _figureType = FigureType.Pawn;

        public FigureType FigureType { get => _figureType ; }
        public FigureColors FigureColor { get ; set ; }

        public List<Position> GetMovableAndCutableBlocks(Position position)
        {
            throw new NotImplementedException();
        }
    }
}
