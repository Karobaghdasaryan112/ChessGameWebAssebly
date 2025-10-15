using SharedResources.ChessGameResource.Models;

namespace ChessGame.Core.Services.Extentions
{
    public static class BoardExtention
    {
        public static Block GetBlockByPosition(this Board board,Position position)
        {
            return Board.BoardBlocks[(int)position.VerticalOrientation][(int)position.HorizontalOrientation];
        }
    }
}
