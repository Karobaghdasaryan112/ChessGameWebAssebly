using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.CriticalValues;
using SharedResources.ChessGameResource.Enums.Orientations;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Models
{
    public class Block : IBlock
    {
        public Position Position { get; set; }
        public IFigure Figure { get; set; }
        public BlockColor BlockColor { get; set; }

        public static Block InitializeBlock(IFigure figure, int i, int j)
        {
            return new Block()
            {
                Figure = figure,
                BlockColor = (BlockColor)((i + j) % 2),
                Position = new Position(
                                   (VerticalOrientation)i,
                                   (HorizontalOrientation)j)
            };
        }

        public void ExploreDirection(
            MovableAndCutablePositions positions,
            int row, int col,
            int rowStep, int colStep)
        {
            while (true)
            {
                row += rowStep;
                col += colStep;

                if ((
                    row == (int)CriticalPositions.lowCriticalValue ||
                    row == (int)CriticalPositions.highCriticalValue ||
                    col == (int)CriticalPositions.lowCriticalValue ||
                    col == (int)CriticalPositions.highCriticalValue)
                    )
                    break;

                var block = Board.GetBlockByPosition(row, col);
                var figure = block.Figure;

                if (figure == null)
                {
                    positions.MovablePositions.Add(new Position(row, col));
                }
                else if (figure.FigureColor != Board.MyColor)
                {
                    positions.CutablePositions.Add(new Position(row, col));
                    row = (int)CriticalPositions.lowCriticalValue;
                    col = (int)CriticalPositions.lowCriticalValue;
                    break;
                }
                else
                {
                    row = (int)CriticalPositions.lowCriticalValue;
                    col = (int)CriticalPositions.lowCriticalValue;
                    break;
                }
            }
        }
    }
}

