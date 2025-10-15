using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;
using System.Collections.Generic;

namespace SharedResources.ChessGameResource.Figures
{
    public class Queen : IFigure
    {
        public FigureType FigureType => FigureType.Queen;
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

            var queenAsBishop = currentBlock.Figure = new Bishop();
            var resultAsBishop = queenAsBishop.GetMovableAndCutableBlocks(position);

            var queenAsRook = currentBlock.Figure = new Rook();
            var resultAsRook = queenAsRook.GetMovableAndCutableBlocks(position);

            result.MovablePositions.AddRange(resultAsBishop.MovablePositions);
            result.CutablePositions.AddRange(resultAsBishop.CutablePositions);

            result.MovablePositions.AddRange(resultAsRook.MovablePositions);
            result.CutablePositions.AddRange(resultAsRook.CutablePositions);

            return result;
        }
    }
}
