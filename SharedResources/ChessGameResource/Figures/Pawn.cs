using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.CriticalValues;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class Pawn : IFigure
    {

        public Pawn() { }

        public FigureType FigureType => FigureType.Pawn;

        public FigureColors FigureColor { get; set; }

        public MovableAndCutablePositions GetMovableAndCutableBlocks(Position position, Board board)
        {
            var result = new MovableAndCutablePositions
            {
                MovableBlock = new List<Block>(),
                CutableBlock = new List<Block>()
            };

            int startRow = (int)position.VerticalOrientation;

            int startCol = (int)position.HorizontalOrientation;

            int stepRow;

            if (FigureColor == FigureColors.Black)
                stepRow = startRow == 1 ? 2 : 1;
            else
                stepRow = startRow == 6 ? -2 : -1;

            AddMovablePositions(startRow, startCol, stepRow, result, board);

            AddCutablePositions(startRow, startCol, -1, result, board);

            AddCutablePositions(startRow, startCol, +1, result, board);

            return result;

        }
        private void AddMovablePositions(int row, int col, int rowStep, MovableAndCutablePositions positions, Board board)
        {
            for (int i = 1; i <= Math.Abs(rowStep); i++)
            {
                var increment = rowStep < 0 ? -1 : 1;
                row += increment;

                if (row >= (int)CriticalPositions.lowCriticalValue && row <= (int)CriticalPositions.highCriticalValue)
                {
                    var block = board.GetBlockByPosition(row, col);

                    var figure = block.Figure;

                    if (figure == null)
                    {
                        block.EventColor = EventColors.Move;
                        positions.MovableBlock.Add(block);
                    }
                    else
                        break;
                }
            }
        }

        private void AddCutablePositions(int row, int col, int columnStep, MovableAndCutablePositions result, Board board)
        {
            col += columnStep;
            var icrement = board.FigureColor == FigureColors.Black ? -1 : +1;
            row += icrement;

            if ((
                row > (int)CriticalPositions.lowCriticalValue &&
                row < (int)CriticalPositions.highCriticalValue &&
                col > (int)CriticalPositions.lowCriticalValue &&
                col < (int)CriticalPositions.highCriticalValue)
                )
            {
                var block = board.GetBlockByPosition(row, col);
                var figure = block.Figure;

                if (figure?.FigureColor != board.FigureColor && figure?.FigureColor != (default))
                {
                    block.EventColor = EventColors.Cut;
                    result.CutableBlock.Add(block);
                }
            }
        }
    }
}
