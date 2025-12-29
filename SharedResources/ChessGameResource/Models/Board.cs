using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Enums.Orientations;
using SharedResources.ChessGameResource.Figures;
using SharedResources.Contracts.ChessGameResourceContracts;

namespace SharedResources.ChessGameResource.Models
{
    /// <summary>
    /// Represents the chessboard in the game.
    /// Handles initialization and retrieval of blocks and chess pieces.
    /// </summary>
    public class Board : ICusotomComparable, ICloneable
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
        public Block[][]? BoardBlocks { get; set; }

        public FigureColors FigureColor { get; set; }

        public Turn Turn = Turn.White;
        public void SwitchTurn()
        {
            Turn =
                Turn == Turn.White ?
                Turn.Black :
                Turn.White;
        }

        public void CreateBoard(FigureColors figureColor = default)
        {
            if (BoardBlocks != default)
                return;

            CreateBlocks(figureColor);
        }

        /// <summary>
        /// Creates and fills the 8x8 board with blocks and places the appropriate chess pieces.
        /// </summary>
        /// <param name="figureColor">The player's chosen figure color</param>
        public void CreateBlocks(FigureColors figureColor = default)
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
        public Block GetBlockByPosition(VerticalOrientation verticalOrientation, HorizontalOrientation horizontalOrientation)
        {
            CreateBoard();
            return BoardBlocks[(int)verticalOrientation][(int)horizontalOrientation];
        }

        /// <summary>
        /// Retrieves a block using a Position object.
        /// </summary>
        public Block GetBlockByPosition(Position position)
        {
            CreateBoard();
            var block = BoardBlocks[(int)position.VerticalOrientation][(int)position.HorizontalOrientation];
            return block;
        }

        /// <summary>
        /// Retrieves a block using a Position object.
        /// </summary>
        public Block GetBlockByPosition(int verticalOrientation, int horizontalOrientation)
        {
            CreateBoard();
            return BoardBlocks[verticalOrientation][horizontalOrientation];
        }

        /// <summary>
        /// Retrieves all blocks on the board that contain a figure matching the specified type and color.
        /// </summary>
        /// <param name="figureType">The type of figure to search for within the board blocks.</param>
        /// <param name="figureColor">The color of the figure to search for within the board blocks.</param>
        /// <returns>A list of blocks containing a figure with the specified type and color. The list is empty if no such blocks
        /// are found.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the board blocks have not been initialized.</exception>

        public List<Block> GetBlockByFigureTypeAndColor(FigureType figureType, FigureColors figureColor)
        {
            if (BoardBlocks is null)
                throw new InvalidOperationException("BoardBlocks is not initialized.");

            var selectedBlocks = BoardBlocks.SelectMany(blocks => blocks.Where(block =>
                block.Figure?.FigureType == figureType && block.Figure?.FigureColor == figureColor));

            var selectedKing = selectedBlocks.ToList();

            if (selectedKing == null)
                throw new InvalidOperationException($"No block found with figure type {figureType} and color {figureColor}.");

            return selectedKing;
        }

        /// <summary>
        /// Compares the current board to another board and returns a list of blocks that differ between them.
        /// </summary>
        /// <remarks>Blocks are compared by their positions in the board. Only blocks that differ in value
        /// are included in the returned list. The comparison assumes both boards are 8x8 in size.</remarks>
        /// <param name="other">The board to compare with the current board. Must not be null and must have the same dimensions as the
        /// current board.</param>
        /// <returns>A list of blocks from the specified board that are not equal to the corresponding blocks in the current
        /// board. The list is empty if all blocks are equal.</returns>
        public List<Block> CompareTo(Board other)
        {
            List<Block> nonEqualBlocks = new();
            var currentBoardBlocks = BoardBlocks;
            var otherBoardBlocks = other.BoardBlocks;
            for (int indexI = 0; indexI < 8; indexI++)
            {
                for (int indexJ = 0; indexJ < 8; indexJ++)
                {
                    if (currentBoardBlocks[indexI][indexJ].CompareTo(otherBoardBlocks[indexI][indexJ]) != 0)
                        nonEqualBlocks.Add(otherBoardBlocks[indexI][indexJ]);
                }
            }

            return nonEqualBlocks;
        }


        /// <summary>
        /// Creates a new object that is a deep copy of the current Board instance.
        /// </summary>
        /// <remarks>The returned object is independent of the original Board. Changes to the cloned Board
        /// or its blocks do not affect the original instance, and vice versa.</remarks>
        /// <returns>A new object that is a deep copy of this Board, including all contained blocks and their state.</returns>
        public object Clone()
        {
            var clonedBoard = new Board(this.FigureColor)
            {
                Turn = this.Turn,
                BoardBlocks = new Block[8][]
            };
            for (int i = 0; i < 8; i++)
            {
                clonedBoard.BoardBlocks[i] = new Block[8];
                for (int j = 0; j < 8; j++)
                {
                    clonedBoard.BoardBlocks[i][j] = (Block)this.BoardBlocks[i][j].Clone();
                }
            }
            return clonedBoard;
        }
    }
} 
