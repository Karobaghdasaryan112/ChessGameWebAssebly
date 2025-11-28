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
        public EventColors EventColor { get; set; }

        public Block() { }

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
            int rowStep, int colStep, Board board)
        {
            while (true)
            {
                row += rowStep;
                col += colStep;

                if ((row <= (int)CriticalPositions.lowCriticalValue || row >= (int)CriticalPositions.highCriticalValue || col <= (int)CriticalPositions.lowCriticalValue || col >= (int)CriticalPositions.highCriticalValue))
                    break;
                if (row < 0 || row > 7 || col < 0 || col > 7)
                {
                    var x = 10;
                }
                var block = board.GetBlockByPosition(row, col);
                var figure = block.Figure;

                if (figure == null)
                {
                    positions.MovableBlock.Add(block);
                    block.EventColor = EventColors.Move;
                }
                else if ((int)figure.FigureColor != (int)board.Turn)
                {
                    positions.CutableBlock.Add(block);
                    block.EventColor = EventColors.Cut;
                    break;
                }
                else
                    break;
            }
        }
    }
}

