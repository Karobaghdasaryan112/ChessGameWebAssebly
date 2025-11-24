using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class Bishop : IFigure
    {
        public Bishop()
        {
            
        }
        public FigureType FigureType => FigureType.Bishop;
        public FigureColors FigureColor { get; set; }

        public MovableAndCutablePositions GetMovableAndCutableBlocks(Position position, Board board)
        {
            var result = new MovableAndCutablePositions
            {
                CutableBlock = new List<Block>(),
                MovableBlock = new List<Block>()
            };

            int startRow = (int)position.VerticalOrientation;
            int startCol = (int)position.HorizontalOrientation;

            var currentBlock = board.GetBlockByPosition(startRow, startCol);


            currentBlock.ExploreDirection(result, startRow, startCol, +1, +1, board);

            currentBlock.ExploreDirection(result, startRow, startCol, +1, -1, board);

            currentBlock.ExploreDirection(result, startRow, startCol, -1, +1, board);

            currentBlock.ExploreDirection(result, startRow, startCol, -1, -1, board);

            return result;
        }
    }
}
