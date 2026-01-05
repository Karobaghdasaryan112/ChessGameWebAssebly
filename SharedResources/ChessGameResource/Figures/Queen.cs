using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class Queen : IFigure
    {
        public Queen() { }
        // StartGame
        public double[][] StartGameQueenTable = new double[8][]
        {
            new double[] { -2, -1, -1, -0, -0, -1, -1, -2 },
            new double[] { -1, 0, 0, 0, 0, 0, 0, -1 },
            new Double[] { -1, 0, 0.5, 0.5, 0.5, 0.5, 0, -1 },
            new double[] { 0, 0, 0.5, 0.5, 0.5, 0.5, 0, 0 },
            new double[] { -0.5, 0, 0.5, 0.5, 0.5, 0.5, 0, -0.5 },
            new double[] { -1, 0.5, 0.5, 0.5, 0.5, 0.5, 0, -1 },
            new double[] { -1, 0, 0.5, 0, 0, 0, 0, -1 },
            new double[] { -2, -1, -1, -0, -0, -1, -1, -2 }
        };

        // MidGame
        public double[][] MidGameQueenTable = new double[8][]
        {
            new double[] { -1, -1, -1, -0, -0, -1, -1, -1 },
            new double[] { -1, 0, 0, 0, 0, 0, 0, -1 },
            new double[] { -1, 0, 0.5, 0.5, 0.5, 0.5, 0, -1 },
            new double[] { 0, 0, 0.5, 0.5, 0.5, 0.5, 0, 0 },
            new double[] { -0.5, 0, 0.5, 0.5, 0.5, 0.5, 0, -0.5 },
            new double[] { -1, 0.5, 0.5, 0.5, 0.5, 0.5, 0, -1 },
            new double[] { -1, 0, 0, 0, 0, 0, 0, -1 },
            new double[] { -1, -1, -1, -0, -0, -1, -1, -1 }
        };

        // EndGame
        public double[][] EndGameQueenTable => StartGameQueenTable; 

        public double[][] MidGameTable => MidGameQueenTable;
        public double[][] StartGameTable => StartGameQueenTable;
        public double[][] EndGameTable => EndGameQueenTable;
        public FigureType FigureType => FigureType.Queen;
        public FigureColors FigureColor { get; set; }
        public bool IsMoves { get; set; }

        public MovableAndCutablePositions GetMovableAndCuttableBlocks(Position position, Board board, Block? kingBlockForCheckCondition)
        {
            var result = new MovableAndCutablePositions
            {
                MovableBlock = [],
                CutableBlock = []
            };

            var startRow = (int)position.VerticalOrientation;
            var startCol = (int)position.HorizontalOrientation;

            var currentBlock = board.GetBlockByPosition(startRow, startCol);

            var queenAsBishop = new Bishop();
            var resultAsBishop = queenAsBishop.GetMovableAndCuttableBlocks(position, board, kingBlockForCheckCondition);

            var queenAsRook = new Rook();
            var resultAsRook = queenAsRook.GetMovableAndCuttableBlocks(position, board, kingBlockForCheckCondition);

            result.MovableBlock.AddRange(resultAsBishop.MovableBlock);
            result.CutableBlock.AddRange(resultAsBishop.CutableBlock);

            result.MovableBlock.AddRange(resultAsRook.MovableBlock);
            result.CutableBlock.AddRange(resultAsRook.CutableBlock);

            return result;
        }

        public string GetFenChar()
        {
            return FigureColor == FigureColors.White ? "Q" : "q";
        }

        public object Clone()
        {
            return new Queen
            {
                FigureColor = this.FigureColor
            };
        }
    }
}
