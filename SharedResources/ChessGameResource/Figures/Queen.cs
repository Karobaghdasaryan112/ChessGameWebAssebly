using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;
using System.Collections.Generic;

namespace SharedResources.ChessGameResource.Figures
{
    public class Queen : IFigure
    {
        public Queen() { }
        public FigureType FigureType => FigureType.Queen;
        public FigureColors FigureColor { get; set; }

        public MovableAndCutablePositions GetMovableAndCutableBlocks(Position position, Board board, Block? kingBlockForCheckCondition)
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
            var resultAsBishop = queenAsBishop.GetMovableAndCutableBlocks(position, board, kingBlockForCheckCondition);

            var queenAsRook = new Rook();
            var resultAsRook = queenAsRook.GetMovableAndCutableBlocks(position, board, kingBlockForCheckCondition);

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
    }
}
