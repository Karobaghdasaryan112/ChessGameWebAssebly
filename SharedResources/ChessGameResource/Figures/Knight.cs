using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.CriticalValues;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Enums.Scores;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class Knight : IFigure
    {
        public Knight()
        {

        }

        private double[][] _startGameKnightTable = new double[8][]
        {
            new double[] { -5, -4, -3, -3, -3, -3, -4, -5 },
            new double[] { -4, -2, 0, 0, 0, 0, -2, -4 },
            new double[] { -3, 0, 1, 2, 2, 1, 0, -3 },
            new double[] { -3, 1, 2, 3, 3, 2, 1, -3 },
            new double[] { -3, 0, 2, 3, 3, 2, 0, -3 },
            new double[] { -3, 1, 1, 2, 2, 1, 1, -3 },
            new double[] { -4, -2, 0, 1, 1, 0, -2, -4 },
            new double[] { -5, -4, -3, -3, -3, -3, -4, -5 }
        };

        private double[][] _midGameKnightTable => _startGameKnightTable;

        private double[][] _endGameKnightTable => _startGameKnightTable; 


        public double[][] MidGameTable => _midGameKnightTable;
        public double[][] StartGameTable => _startGameKnightTable;
        public double[][] EndGameTable => _endGameKnightTable;


        public FigureType FigureType => FigureType.Knight;
        public FigureColors FigureColor { get; set; }


        public MovableAndCutablePositions GetMovableAndCuttableBlocks(Position position, Board board, Block? kingBlockForCheckCondition)
        {
            var result = new MovableAndCutablePositions
            {
                CutableBlock = [],
                MovableBlock = []
            };

            var startRow = (int)position.VerticalOrientation;
            var startCol = (int)position.HorizontalOrientation;

            var kingBlock = kingBlockForCheckCondition;

            var row = startRow;
            var col = startCol;

            AddPositions(row, col, +1, +2, result, board);

            AddPositions(row, col, +1, -2, result, board);

            AddPositions(row, col, -1, +2, result, board);

            AddPositions(row, col, -1, -2, result, board);

            AddPositions(row, col, +2, -1, result, board);

            AddPositions(row, col, +2, +1, result, board);

            AddPositions(row, col, -2, -1, result, board);

            AddPositions(row, col, -2, +1, result, board);

            return result;
        }

        public string GetFenChar()
        {
            return FigureColor == FigureColors.White ? "N" : "n";
        }

        private void AddPositions(int row, int col, int rowStep, int colStep, MovableAndCutablePositions positions, Board board)
        {
            try
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
                    positions.MovableBlock.Add(block);
                    block.EventColor = EventColors.Move;
                }
                else if ((int)figure.FigureColor != (int)board.Turn)
                {
                    block.EventColor = EventColors.Cut;
                    positions.CutableBlock?.Add(block);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public object Clone()
        {
            return new Knight
            {
                FigureColor = this.FigureColor
            };
        }
    }
}
