using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.CriticalValues;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class Pawn : IFigure
    {
        public FigureType FigureType => FigureType.Pawn;
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

            var stepRow = startRow == 6 ? -2 : -1;

            AddMovablePositions(startRow, startCol, stepRow, result);

            AddCutablePositions(startRow, startCol, -1, result);

            AddCutablePositions(startRow, startCol, +1, result);

            return result;

        }
        private void AddMovablePositions(int row, int col, int rowStep, MovableAndCutablePositions positions)
        {
            for (int i = 1; i <= rowStep; i++)
            {
                row += i;

                if (row >= (int)CriticalPositions.lowCriticalValue && row <= (int)CriticalPositions.highCriticalValue)
                {
                    var block = Board.GetBlockByPosition(row, col);
                    var figure = block.Figure;

                    if (figure == null)
                        positions.MovablePositions.Add(new Position(row, col));
                    else
                        break;
                }
            }
        }

        private void AddCutablePositions(int row, int col, int columnStep, MovableAndCutablePositions result)
        {
            col += columnStep;
            row += -1;

            if ((
                row != (int)CriticalPositions.lowCriticalValue ||
                row != (int)CriticalPositions.highCriticalValue ||
                col != (int)CriticalPositions.lowCriticalValue ||
                col != (int)CriticalPositions.highCriticalValue)
                )
            {
                var block = Board.GetBlockByPosition(row, col);
                var figure = block.Figure;

                if (figure?.FigureColor != Board.MyColor)
                    result.CutablePositions.Add(new Position(row, col));
            }
        }
    }
}
