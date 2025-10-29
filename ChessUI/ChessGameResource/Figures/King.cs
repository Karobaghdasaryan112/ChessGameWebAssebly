using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.CriticalValues;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class King : IFigure
    {
        public FigureType FigureType => FigureType.King;
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


            var row = startRow;
            var col = startCol;

            AddPositions(row, col, +1, 0, result);

            AddPositions(row, col, -1, 0, result);

            AddPositions(row, col, 0, -1, result);

            AddPositions(row, col, 0, +1, result);

            AddPositions(row, col, +1, -1, result);

            AddPositions(row, col, -1, -1, result);

            AddPositions(row, col, +1, +1, result);

            AddPositions(row, col, -1, +1, result);

            return result;
        }

        private void AddPositions(int row, int col, int rowStep, int colStep, MovableAndCutablePositions positions)
        {
            row += rowStep;
            col += colStep;

            if ((
                row != (int)CriticalPositions.lowCriticalValue ||
                row != (int)CriticalPositions.highCriticalValue ||
                col != (int)CriticalPositions.lowCriticalValue ||
                col != (int)CriticalPositions.highCriticalValue)
                )
            {
                var block = Board.GetBlockByPosition(row, col);
                var figure = block.Figure;

                if (figure == null)
                    positions.MovablePositions.Add(new Position(row, col));
                else if (figure.FigureColor != Board.MyColor)
                    positions.CutablePositions.Add(new Position(row, col));
            }
        }
    }
}
