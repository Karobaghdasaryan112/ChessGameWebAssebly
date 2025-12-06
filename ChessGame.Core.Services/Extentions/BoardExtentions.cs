using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;

namespace ChessGame.Core.Services.Extentions
{
    public static class BoardExtentions
    {
        public static void ResetEventableBlocks(this Board board)
        {
            //reset the previous selected Blocks(Movable and cuttable)
            var preventableBoardBlocks =
                board.BoardBlocks!.
                    SelectMany(blockI =>
                        blockI.Where(blockJ =>
                            blockJ.EventColor is
                                EventColors.Cut or
                                EventColors.Move).
                            ToArray());

            foreach (var preventableBoardBlock in preventableBoardBlocks)
                preventableBoardBlock.EventColor = EventColors.None;
        }
    }
}
