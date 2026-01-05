using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;

namespace ChessGame.Core.Services.Extentions
{
    public static class BoardExtentions
    {
        public static void ResetEventableBlocks(this Board board)
        {
            var preventableBoardBlocks =
                board.BoardBlocks!.
                    SelectMany(blockI =>
                        blockI.Where(blockJ =>
                            blockJ.EventColor is
                                EventColors.Cut or
                                EventColors.Move or
                                EventColors.Castle).
                            ToArray());

            foreach (var preventableBoardBlock in preventableBoardBlocks)
                preventableBoardBlock.EventColor = EventColors.None;
        }
    }
}
