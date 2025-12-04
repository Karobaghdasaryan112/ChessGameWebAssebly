using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class Bishop : IFigure
    {
        public Bishop() { }
        public FigureType FigureType => FigureType.Bishop;
        public FigureColors FigureColor { get; set; }

        public MovableAndCutablePositions GetMovableAndCutableBlocks(Position position, Board board, Block? kingBlockForCheckCondition)
        {
            var result = new MovableAndCutablePositions
            {
                CutableBlock = [],
                MovableBlock = []
            };

            var startRow = (int)position.VerticalOrientation;
            var startCol = (int)position.HorizontalOrientation;

            var currentBlock = kingBlockForCheckCondition ?? board.GetBlockByPosition(startRow, startCol);


            currentBlock.ExploreDirection(result, startRow, startCol, +1, +1, board);

            currentBlock.ExploreDirection(result, startRow, startCol, +1, -1, board);

            currentBlock.ExploreDirection(result, startRow, startCol, -1, +1, board);

            currentBlock.ExploreDirection(result, startRow, startCol, -1, -1, board);

            return result;
        }

        public string GetFenChar()
        {
            return FigureColor == FigureColors.White ? "B" : "b";
        }
    }
}
