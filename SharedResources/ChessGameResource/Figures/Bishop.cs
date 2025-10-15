using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class Bishop : IFigure
    {
        public FigureType FigureType => FigureType.Bishop;
        public FigureColors FigureColor { get; set; }

        public MovableAndCutablePositions GetMovableAndCutableBlocks(Position position)
        {
            var result = new MovableAndCutablePositions
            {
                MovablePositions = new List<Position>(),
                CutablePositions = new List<Position>()
            };

            int startRow = (int)position.VerticalOrientation;
            int startCol = (int)position.HorizontalOrientation;

            var currentBlock = Board.GetBlockByPosition(startRow, startCol);


            currentBlock.ExploreDirection(result, startRow, startCol, +1, +1);

            currentBlock.ExploreDirection(result, startRow, startCol, +1, -1);

            currentBlock.ExploreDirection(result, startRow, startCol, -1, +1);

            currentBlock.ExploreDirection(result, startRow, startCol, -1, -1);

            return result;
        }
    }
}
