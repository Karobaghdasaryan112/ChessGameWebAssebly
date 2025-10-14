using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class Pawn : IFigure
    {
        private FigureType _figureType => FigureType.Pawn;

        public FigureType FigureType { get => _figureType; set { FigureType = value; } }
        public FigureColors FigureColor { get; set; }
        public string FigurePath => $"{FigureType}{FigureColor}.png";

        public Pawn()
        {
        }

        public List<Position> GetMovableAndCutableBlocks(Board board,Position position)
        {
            throw new NotImplementedException();
        }
    }
}
