using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using System.Text;

namespace ChessGame.Core.Services.Extentions
{
    public static class ConversionExtention
    {
        public static string FromBoardToFen(this Board board)
        {

            StringBuilder fenBuilder = new StringBuilder();
            for (int row = 0; row < 8; row++)
            {
                int emptyCount = 0;
                for (int col = 0; col < 8; col++)
                {
                    if (board.BoardBlocks != null)
                    {
                        var block = board.GetBlockByPosition(row, col);

                        if (block is { Figure: not null })
                        {
                            if (emptyCount > 0)
                            {
                                fenBuilder.Append(emptyCount);
                                emptyCount = 0;
                            }

                            fenBuilder.Append(block.Figure.GetFenChar());
                        }
                        else
                        {
                            emptyCount++;
                        }
                    }
                }

                if (emptyCount > 0)
                {
                    fenBuilder.Append(emptyCount);
                }

                if (row < 7)
                {
                    fenBuilder.Append('/');
                }
            }

            return fenBuilder.ToString();
        }

        public static Board FromFenToBoard(this string fen)
        {
            Board board = new Board(FigureColors.White);
            board.BoardBlocks = new Block[8][];
            string[] rows = fen.Split('/');
            for (int row = 0; row < 8; row++)
            {
                board.BoardBlocks[row] = new Block[8];
                int col = 0;
                foreach (char c in rows[row])
                {
                    if (char.IsDigit(c))
                    {
                        int emptyCount = c - '0';
                        for (int i = 0; i < emptyCount; i++)
                        {
                            board.BoardBlocks[row][col] = Block.InitializeBlock(null, row, col);
                            col++;
                        }
                    }
                    else
                    {
                        var figure = FigureExtensions.FromFenCharToFigure(c);
                        board.BoardBlocks[row][col] = Block.InitializeBlock(figure, row, col);
                        col++;
                    }
                }
            }

            return board;
        }
    }
}
