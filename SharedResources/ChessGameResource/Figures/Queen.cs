using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;
using System.Collections.Generic;

namespace SharedResources.ChessGameResource.Figures
{
    public class Queen : IFigure
    {
        public Queen()
        {
            
        }   
        public FigureType FigureType => FigureType.Queen;
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

            var currentBlock = board.GetBlockByPosition(startRow, startCol);

            var queenAsBishop = currentBlock.Figure = new Bishop();
            var resultAsBishop = queenAsBishop.GetMovableAndCutableBlocks(position, board);

            var queenAsRook = currentBlock.Figure = new Rook();
            var resultAsRook = queenAsRook.GetMovableAndCutableBlocks(position, board);

            result.MovableBlock.AddRange(resultAsBishop.MovableBlock);
            result.CutableBlock.AddRange(resultAsBishop.CutableBlock);

            result.MovableBlock.AddRange(resultAsRook.MovableBlock);
            result.CutableBlock.AddRange(resultAsRook.CutableBlock);

            return result;
        }
    }
}
