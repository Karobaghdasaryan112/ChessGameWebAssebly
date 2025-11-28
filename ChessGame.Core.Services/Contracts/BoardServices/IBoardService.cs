using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace ChessGame.Core.Services.Contracts.BoardServices
{
    public interface IBoardService
    {
        Task<Guid> InitializeBoardAsync(Guid player1Id, Guid player2Id);
        Task<bool> SubmitMoveAsync(Guid gameId, Position currentPosition, Position movePosition, Board currentBoardState);
        Task<Block> CanClick(FigureColors currentColor, Block currentBlock, ClickedBlockInformationDTO previusBlockInformationDTO, Board currentBoard);
        Task<bool> IsKingCheckedAsync(Board currentBoard,Turn chosenColor);
    }
}
