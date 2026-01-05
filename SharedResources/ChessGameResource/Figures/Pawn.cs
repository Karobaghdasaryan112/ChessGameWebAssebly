using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.CriticalValues;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Enums.Scores;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class Pawn : IFigure
    {
        public Pawn() { }

        public double[][] _startGameTable = new double[8][]
        {
            new double[] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new double[] { 5, 5, 5, 5, 5, 5, 5, 5 },
            new double[] { 1, 1, 2, 3, 3, 2, 1, 1 },
            new double[] { 0, 0, 1, 2, 2, 1, 0, 0 },
            new double[] { 0, 0, 0, 2, 2, 0, 0, 0 },
            new double[] { 0, 0, 0, -1, -1, 0, 0, 0 },
            new double[] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new double[] { 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        public double[][] _midGameTable = new double[8][]
        {
            new double[] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new double[] { 5, 5, 5, 5, 5, 5, 5, 5 },
            new double[] { 2, 2, 3, 4, 4, 3, 2, 2 },
            new double[] { 1, 1, 2, 3, 3, 2, 1, 1 },
            new double[] { 0, 0, 1, 2, 2, 1, 0, 0 },
            new double[] { 0, 0, 0, 1, 1, 0, 0, 0 },
            new double[] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new double[] { 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        public double[][] _endGameTable = new double[8][]
        {
            new double[] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new double[] { 10, 10, 10, 10, 10, 10, 10, 10 },
            new double[] { 5, 5, 6, 8, 8, 6, 5, 5 },
            new double[] { 2, 2, 4, 6, 6, 4, 2, 2 },
            new double[] { 0, 0, 2, 4, 4, 2, 0, 0 },
            new double[] { 0, 0, 0, 2, 2, 0, 0, 0 },
            new double[] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new double[] { 0, 0, 0, 0, 0, 0, 0, 0 }
        };


        public double[][] MidGameTable => _midGameTable;
        public double[][] StartGameTable => _startGameTable;
        public double[][] EndGameTable => _endGameTable;
        public FigureType FigureType => FigureType.Pawn;

        public FigureColors FigureColor { get; set; }
        public bool IsMoves { get; set; }

        public MovableAndCutablePositions GetMovableAndCuttableBlocks(Position position, Board board, Block? kingBlockForCheckCondition)
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

            try
            {
                AddMovablePositions(startRow, startCol, stepRow, result, board);

                AddCutablePositions(startRow, startCol, -1, result, board);

                AddCutablePositions(startRow, startCol, +1, result, board);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            return result;

        }

        public string GetFenChar()
        {
            return FigureColor == FigureColors.White ? "P" : "p";
        }

        private void AddMovablePositions(int row, int col, int rowStep, MovableAndCutablePositions positions, Board board)
        {
            for (int i = 1; i <= Math.Abs(rowStep); i++)
            {
                var increment = rowStep < 0 ? -1 : 1;
                row += increment;

                if (row <= (int)CriticalPositions.lowCriticalValue ||
                    row >= (int)CriticalPositions.highCriticalValue) continue;

                var block = board.GetBlockByPosition(row, col);

                var figure = block.Figure;

                if (figure == null)
                {
                    block.EventColor = EventColors.Move;
                    positions.MovableBlock?.Add(block);
                }
                else
                    break;
            }
        }

        private void AddCutablePositions(int row, int col, int columnStep, MovableAndCutablePositions result, Board board)
        {
            col += columnStep;
            var increment = FigureColor == FigureColors.Black ? +1 : -1;
            row += increment;

            if ((
                row is > (int)CriticalPositions.lowCriticalValue and < (int)CriticalPositions.highCriticalValue &&
                col is > (int)CriticalPositions.lowCriticalValue and < (int)CriticalPositions.highCriticalValue)
                )
            {
                var block = board.GetBlockByPosition(row, col);
                var figure = block.Figure;

                if (figure?.FigureColor == (null)) return;

                if ((int)figure?.FigureColor! == (int)board.Turn) return;

                block.EventColor = EventColors.Cut;
                result.CutableBlock?.Add(block);
            }
        }

        public object Clone()
        {
            return new Pawn
            {
                FigureColor = this.FigureColor
            };
        }
    }
}
