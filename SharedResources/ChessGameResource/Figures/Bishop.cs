using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class Bishop : IFigure
    {
        public Bishop() { }


        public double[][] MidGameTable => _midGameBishopTable;
        public double[][] StartGameTable => _startGameBishopTable;
        public double[][] EndGameTable => _endGameBishopTable;


        private double[][] _startGameBishopTable = new double[8][]
        {
            new double[] { -2, -1, -1, -1, -1, -1, -1, -2 },
            new double[] { -1, 0, 0, 0, 0, 0, 0, -1 },
            new double[] { -1, 0, 0,5, 1, 1, 0,5, 0, -1 },
            new double[] { -1, 0,5, 0,5, 1, 1, 0,5, 0,5, -1 },
            new double[] { -1, 0, 1, 1, 1, 1, 0, -1 },
            new double[] { -1, 1, 1, 1, 1, 1, 1, -1 },
            new double[] { -1, 0,5, 0, 0, 0, 0, 0,5, -1 },
            new double[] { -2, -1, -1, -1, -1, -1, -1, -2 }
        };

        // MidGame: чуть больше бонуса на центр
        private double[][] _midGameBishopTable => _startGameBishopTable;

        // EndGame: слон сильнее
        private double[][] _endGameBishopTable => _startGameBishopTable;

        public FigureType FigureType => FigureType.Bishop;
        public FigureColors FigureColor { get; set; }
        public bool IsMoves { get; set; }

        public MovableAndCutablePositions GetMovableAndCuttableBlocks(Position position, Board board, Block? kingBlockForCheckCondition)
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

        public object Clone()
        {
            return new Bishop
            {
                FigureColor = this.FigureColor
            };
        }
    }
}
