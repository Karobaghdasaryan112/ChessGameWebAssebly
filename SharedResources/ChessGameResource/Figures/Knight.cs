using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.CriticalValues;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Figures
{
    public class Knight : IFigure
    {
        public Knight()
        {
            
        }
        public FigureType FigureType => FigureType.Knight;
        public FigureColors FigureColor { get; set; }

        public MovableAndCutablePositions GetMovableAndCutableBlocks(Position position, Board board, Block? kingBlockForCheckCondition)
        {
            var result = new MovableAndCutablePositions
            {
                CutableBlock = new List<Block>(),
                MovableBlock = new List<Block>()
            };

            int startRow = (int)position.VerticalOrientation;
            int startCol = (int)position.HorizontalOrientation;

            Block? kingBlock = kingBlockForCheckCondition != default(Block) ? kingBlockForCheckCondition : null;

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

        private void AddPositions(int row, int col, int rowStep, int colStep, MovableAndCutablePositions positions,Board board)
        {
            try
            {
                row += rowStep;
                col += colStep;

                if ((
                        row > (int)CriticalPositions.lowCriticalValue &&
                        row < (int)CriticalPositions.highCriticalValue &&
                        col > (int)CriticalPositions.lowCriticalValue &&
                        col < (int)CriticalPositions.highCriticalValue)
                   )
                {
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
                        positions.CutableBlock.Add(block);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
    }
}
