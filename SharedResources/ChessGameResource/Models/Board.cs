using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.Orientations;
using SharedResources.ChessGameResource.Figures;
using System.Text.Json.Serialization;

namespace SharedResources.ChessGameResource.Models
{
    /// <summary>
    /// Represents the chessboard in the game.
    /// Handles initialization and retrieval of blocks and chess pieces.
    /// </summary>
    public class Board
    {
        public Board(FigureColors figureColor)
        {
            if (BoardBlocks != default)
                return;

            CreateBoard(figureColor);
            FigureColor =
                figureColor == default ?
                FigureColors.White :
                figureColor;
        }

        public Block[][] GetBlocks { get; set; }

        /// <summary>
        /// 8x8 grid representing the chess board. Each Block may contain a chess piece or be empty.
        /// </summary>
        public  Block[][]? BoardBlocks { get; set; }

        public  FigureColors FigureColor { get; set; }

        public  void CreateBoard(FigureColors figureColor = default)
        {
            if (BoardBlocks != default)
                return;

            CreateBlocks(figureColor);
        }

        /// <summary>
        /// Creates and fills the 8x8 board with blocks and places the appropriate chess pieces.
        /// </summary>
        /// <param name="figureColor">The player's chosen figure color</param>
        public  void CreateBlocks(FigureColors figureColor = default)
        {

            BoardBlocks = new Block[8][];

            var opponentFigureColor = FigureColors.Black;
            figureColor = FigureColors.White;

            var realFigureColor = default(FigureColors);

            for (int i = 0; i <= 7; i++)
            {
                realFigureColor = i < 2 ? opponentFigureColor : figureColor;

                BoardBlocks[i] = new Block[8];
                for (int j = 0; j <= 7; j++)
                {
                    BlockColor blockColor = (BlockColor)((i + j) % 2);
                    if (i == 0 || i == 7)
                    {
                        if (j == 0 || j == 7)
                            BoardBlocks[i][j] = Block.InitializeBlock(new Rook() { FigureColor = realFigureColor }, i, j);

                        if (j == 1 || j == 6)
                            BoardBlocks[i][j] = Block.InitializeBlock(new Knight() { FigureColor = realFigureColor }, i, j);

                        if (j == 2 || j == 5)
                            BoardBlocks[i][j] = Block.InitializeBlock(new Bishop() { FigureColor = realFigureColor }, i, j);

                        if (j == 3)
                            BoardBlocks[i][j] = Block.InitializeBlock(new Queen() { FigureColor = realFigureColor }, i, j);

                        if (j == 4)
                            BoardBlocks[i][j] = Block.InitializeBlock(new King() { FigureColor = realFigureColor }, i, j);
                    }
                    else if (i == 1 || i == 6)
                        BoardBlocks[i][j] = Block.InitializeBlock(new Pawn() { FigureColor = realFigureColor }, i, j);
                    else
                        BoardBlocks[i][j] = Block.InitializeBlock(default, i, j);
                }
            }

        }
        /// <summary>
        /// Retrieves a block using vertical and horizontal enum coordinates.
        /// </summary>
        public  Block GetBlockByPosition(VerticalOrientation verticalOrientation, HorizontalOrientation horizontalOrientation)
        {
            CreateBoard();
            return BoardBlocks[(int)verticalOrientation][(int)horizontalOrientation];
        }

        /// <summary>
        /// Retrieves a block using a Position object.
        /// </summary>
        public  Block GetBlockByPosition(Position position)
        {
            CreateBoard();
            return BoardBlocks[(int)position.VerticalOrientation][(int)position.HorizontalOrientation];
        }

        /// <summary>
        /// Retrieves a block using a Position object.
        /// </summary>
        public  Block GetBlockByPosition(int verticalOrientation, int horizontalOrientation)
        {
            CreateBoard();
            return BoardBlocks[verticalOrientation][horizontalOrientation];
        }
    }
}
