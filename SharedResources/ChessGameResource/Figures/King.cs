using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.CriticalValues;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Enums.Scores;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class King : IFigure
    {
        public King()
        {

        }
        public FigureType FigureType => FigureType.King;
        public FigureColors FigureColor { get; set; }
        public FigureScores FigureScore => FigureScores.King;

        public MovableAndCutablePositions GetMovableAndCuttableBlocks(Position position, Board board, Block? kingBlockForCheckCondition)
        {
            var result = new MovableAndCutablePositions
            {
                CutableBlock = new List<Block>(),
                MovableBlock = new List<Block>()
            };

            var startRow = (int)position.VerticalOrientation;
            var startCol = (int)position.HorizontalOrientation;

            var currentBlockForCheckCondition = kingBlockForCheckCondition;

            var row = startRow;
            var col = startCol;

            AddPositions(row, col, +1, 0, result, board);

            AddPositions(row, col, -1, 0, result, board);

            AddPositions(row, col, 0, -1, result, board);

            AddPositions(row, col, 0, +1, result, board);

            AddPositions(row, col, +1, -1, result, board);

            AddPositions(row, col, -1, -1, result, board);

            AddPositions(row, col, +1, +1, result, board);

            AddPositions(row, col, -1, +1, result, board);

            return result;
        }

        public string GetFenChar()
        {
            return FigureColor == FigureColors.White ? "K" : "k";
        }

        private void AddPositions(int row, int col, int rowStep, int colStep, MovableAndCutablePositions positions, Board board)
        {
            row += rowStep;
            col += colStep;

            if ((row <= (int)CriticalPositions.lowCriticalValue ||
                 row >= (int)CriticalPositions.highCriticalValue ||
                 col <= (int)CriticalPositions.lowCriticalValue ||
                 col >= (int)CriticalPositions.highCriticalValue)) return;
            var block = board.GetBlockByPosition(row, col);
            var figure = block.Figure;

            if (figure == null)
            {
                block.EventColor = EventColors.Move;
                positions.MovableBlock?.Add(block);
            }
            else if ((int)figure.FigureColor != (int)board.Turn)
            {
                block.EventColor = EventColors.Cut;
                positions.CutableBlock?.Add(block);
            }
        }

        public object Clone()
        {
            return new King
            {
                FigureColor = this.FigureColor
            };
        }
    }
}
