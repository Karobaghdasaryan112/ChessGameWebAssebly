using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Repositories;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Figures;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Net;
using BoardInitializeRequestDTO = SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs.BoardInitializeRequestDTO;
using SubmitMoveRequestDTO = SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs.SubmitMoveRequestDTO;
using SubmitMoveResponseDTO = SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs.SubmitMoveResponseDTO;

namespace ChessGame.Core.Services.Services.BoardService
{
    public class BoardService(
        ILogger<BoardService> logger,
        IChessGameRepository chessGameRepository,
        IChessGameHistoryRepository chessGameHistoryRepository)
        : IBoardService
    {
        private readonly IChessGameHistoryRepository _chessGameHistoryRepository = chessGameHistoryRepository;


        public async Task<ConnectionResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>>
            InitializeBoardAsync(ConnectionRequestDTO<BoardInitializeRequestDTO> connectionRequestDto)
        {

            var isCreated = await chessGameRepository.CreateGame(connectionRequestDto.Data.Player1Id,
                connectionRequestDto.Data.Player2Id);

            if (!isCreated)
            {
                logger.LogError("Failed to create a new game between {Player1} and {Player2}",
                    connectionRequestDto.Data.Player1Id, connectionRequestDto.Data.Player2Id);
                return ConnectionResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new BoardInitializeResponseDTO()
                    {
                        GameId = Guid.Empty
                    }, ChessGameResponseMessage.GameCreationFailed, HttpStatusCode.BadRequest,
                    [
                        "Failed to create a new game between {Player1} and {Player2}",
                        connectionRequestDto.Data.Player1Id.ToString(), connectionRequestDto.Data.Player2Id.ToString()
                    ]);
            }

            var gameId = await chessGameRepository.GetGameIdByPlayers(connectionRequestDto.Data.Player1Id,
                connectionRequestDto.Data.Player2Id);
            if (gameId == Guid.Empty)
            {
                logger.LogError("Failed to retrieve game ID for players {Player1} and {Player2}",
                    connectionRequestDto.Data.Player1Id, connectionRequestDto.Data.Player2Id);
                return ConnectionResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new BoardInitializeResponseDTO()
                    {
                        GameId = Guid.Empty
                    }, ChessGameResponseMessage.GameCreationFailed, HttpStatusCode.BadRequest,
                    [
                        "Failed to retrieve game ID for players {Player1} and {Player2}",
                        connectionRequestDto.Data.Player1Id.ToString(), connectionRequestDto.Data.Player2Id.ToString()
                    ]);
            }
            else
                logger.LogInformation("Game successfully created between {Player1} and {Player2}",
                    connectionRequestDto.Data.Player1Id, connectionRequestDto.Data.Player2Id);

            return ConnectionResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new BoardInitializeResponseDTO()
                {
                    GameId = gameId
                },
                ChessGameResponseMessage.GameCreated,
                HttpStatusCode.Created);
        }


        public async Task<ConnectionResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>> SubmitMoveAsync(
            SubmitMoveRequestDTO submitMoveRequestDto)
        {
            //Data From RequestDto
            var fromPosition = submitMoveRequestDto.From;
            var toPosition = submitMoveRequestDto.To;
            var gameId = submitMoveRequestDto.GameId;
            var currentBoardState = submitMoveRequestDto.CurrentBoardState;

            //Initialize Response DTO
            var connectionResponse = new ConnectionResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>()
            {
                Data = new SubmitMoveResponseDTO()
                {
                    IsKingChecked = false,
                    IsKingMate = false,
                    IsMoveSuccess = true
                },
                IsSuccess = true
            };

            var fromBlock = currentBoardState.GetBlockByPosition(fromPosition!);
            var toBlock = currentBoardState.GetBlockByPosition(toPosition!);

            if (fromBlock?.Figure == null)
            {
                logger.LogWarning("No figure found at position {Position} in game {GameId}", fromPosition, gameId);

                connectionResponse.Data.IsMoveSuccess = false;
                connectionResponse.Errors =
                [
                    $"If there is no figure at the from-{fromBlock.Position.VerticalOrientation}{fromBlock.Position.HorizontalOrientation} position"
                ];
                connectionResponse.IsSuccess = false;
                return connectionResponse;
            }

            //Make the Move
            //Store the figure at the toBlock temporarily
            var toBlockTemp = toBlock.Figure;

            //Move the figure from fromBlock to toBlock
            toBlock.Figure = fromBlock.Figure;
            fromBlock.Figure = default;

            logger.LogInformation("Move submitted in game {GameId} from {FromPosition} to {ToPosition}", gameId,
                fromPosition, toPosition);

            //Check if king is in check after the move
            //If king is in check, return false 
            if (!await IsKingCheckedAsync(currentBoardState, currentBoardState.Turn))
                return connectionResponse;

            //Implement IsKing Mate state
            //TO DO: If there is Mate state then put the IsMateState to True --- connectionResponse.Data.IsKingMate = true;
            logger.LogWarning(
                "Move from {FromPosition} to {ToPosition} in game {GameId} would leave king in check",
                fromPosition, toPosition, gameId);

            //Revert the Move
            fromBlock.Figure = toBlock.Figure;
            toBlock.Figure = toBlockTemp;

            connectionResponse.Data.IsKingChecked = true;

            logger.LogInformation("Move revert in game {GameId} from {FromPosition} to {ToPosition}", gameId,
                fromPosition, toPosition);

            return connectionResponse;
        }



        //Response Block
        //Request FigureColors , Block CurrentBlock
        public async Task<ConnectionResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>> CanClick(
            ConnectionRequestDTO<CanClickRequestDTO> connectionRequestDto)
        {
            //Request Data
            var figureColor = connectionRequestDto.Data.FigureColor;
            var currentBlock = connectionRequestDto.Data.CurrentBlock;
            var previusBlockInformationDTO = connectionRequestDto.Data.ClickedBlockInformationDto;
            var currentBoard = connectionRequestDto.Data.CurrentBoardBoardState!;


            if ((int)figureColor !=
                (int)currentBoard.Turn)
            {
                logger.LogWarning("It's not the turn of player with color {Color}",
                    figureColor);

                return await Task.FromResult(
                    ConnectionResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        new CanClickResponseDTO()
                        {
                            ClickedBlock = null
                        },
                        ChessGameResponseMessage.InvalidMove,
                        HttpStatusCode.BadRequest,
                        ["It's not the turn of the player"]));
            }

            var currentBlockFromServer = currentBoard.GetBlockByPosition(currentBlock.Position);

            //if the current player is the same color as the figure on the clicked block and previusly clicked block is null
            if (currentBlock.Figure != null &&
                currentBlock.Figure.FigureColor == figureColor)
            {
                logger.LogInformation("Player with color {Color} clicked on their own figure at position {Position}",
                    figureColor,
                    currentBlock.Position);

                return await Task.FromResult(
                    ConnectionResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        new CanClickResponseDTO()
                        {
                            ClickedBlock = currentBlockFromServer
                        },
                        ChessGameResponseMessage.MoveSuccessful, HttpStatusCode.Accepted));
            }

            //if the current player is clicked previusly and now clicked on a movable or cutable position

            if (previusBlockInformationDTO?.ClickedPosition == null ||
                (currentBlockFromServer.EventColor != EventColors.Cut &&
                 currentBlockFromServer.EventColor != EventColors.Move))
                return await Task.FromResult(
                    ConnectionResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        new CanClickResponseDTO()
                        {
                            ClickedBlock = null
                        },
                        ChessGameResponseMessage.InvalidMove,
                        HttpStatusCode.BadRequest,
                        ["It's not the turn of the player"]));

            logger.LogInformation("Player with color {Color} is attempting to move from {FromPosition} to {ToPosition}",
                figureColor,
                previusBlockInformationDTO.ClickedPosition,
                currentBlock.Position);

            return await Task.FromResult(
                ConnectionResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    new CanClickResponseDTO()
                    {
                        ClickedBlock = currentBlockFromServer
                    },
                    ChessGameResponseMessage.MoveSuccessful, HttpStatusCode.Accepted));

        }




        public async Task<bool> IsKingCheckedAsync(Board currentBoard, Turn chosenColor)
        {
            var myColor = (FigureColors)chosenColor;
            var kingBlock = currentBoard.GetBlockByFigureTypeAndColor(FigureType.King, myColor).First();

            if (await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Queen, myColor, currentBoard,
                    [FigureType.Queen]) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Rook, myColor, currentBoard,
                    [FigureType.Rook, FigureType.Queen]) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Bishop, myColor, currentBoard,
                    [FigureType.Queen, FigureType.Bishop]) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Knight, myColor, currentBoard,
                    [FigureType.Knight]) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.King, myColor, currentBoard,
                    [FigureType.King]) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Pawn, myColor, currentBoard,
                    [FigureType.Pawn, FigureType.Bishop, FigureType.King, FigureType.Queen]))
                return await Task.FromResult(true);

            return await Task.FromResult(false);
        }

        //private methods
        //if king is in check by a specific figure type, we create a clone of the king block and assign the figure type to it
        // then we get the movable and cuttable blocks of that figure type from the king's position
        // if any of those blocks contain an opponent's figure of the specified type, the king is in check
        // we log the information about the check event for debugging purposes
        // this method is generic and can be used for any figure type
        //Generic method to check if king is checked by a specific figure type
        private async Task<bool> IsKingCheckedBy<TFigureType>(Block kingBlock, TFigureType figureType,
            FigureColors myColor, Board currentBoard, List<FigureType> figureTypes) where TFigureType : Enum
        {
            if (!Enum.IsDefined(typeof(TFigureType), figureType))
                return false;

            var kingBlockClone = new Block
            {
                Position = kingBlock.Position,
                Figure = figureType switch
                {
                    FigureType.Rook => new Rook() { FigureColor = myColor },
                    FigureType.Bishop => new Bishop() { FigureColor = myColor },
                    FigureType.King => new King() { FigureColor = myColor },
                    FigureType.Knight => new Knight() { FigureColor = myColor },
                    FigureType.Pawn => new Pawn() { FigureColor = myColor },
                    FigureType.Queen => new Queen() { FigureColor = myColor },
                    _ => throw new ArgumentException()
                }
            };


            var possibleMovableAndCuttable =
                kingBlockClone.Figure.GetMovableAndCutableBlocks(kingBlockClone.Position, currentBoard, kingBlockClone);
            if (possibleMovableAndCuttable.CutableBlock.Count() == 0)
                return await Task.FromResult(false);

            var figuresForCheck = possibleMovableAndCuttable.CutableBlock.Where(block =>
                figureTypes.Contains<FigureType>(block.Figure.FigureType));

            if (!figuresForCheck.Any())
                return await Task.FromResult(false);

            foreach (var figureForCheck in figuresForCheck)
            {
                logger.LogInformation("King of color {Color} is in check by figure at position {Position}",
                    myColor, figureForCheck.Position);
            }

            return await Task.FromResult(true);

        }

        public void ResetEventableBlocks(Board gameState)
        {
            //reset the previous selected Blocks(Movable and cuttable)
            var preventableBoardBlocks = gameState.BoardBlocks!.SelectMany(blockI => blockI.Where(blockJ => blockJ.EventColor is EventColors.Cut or EventColors.Move).ToArray());

            foreach (var preventableBoardBlock in preventableBoardBlocks)
                preventableBoardBlock.EventColor = EventColors.None;
        }


        //TO DO: Implement King Mate Logic
        //Placeholder method for King Mate logic
        // This method should determine if the king is in a checkmate position

        public async Task<bool> IsKingMateAsync(Board? currentBoard, Guid gameId, Turn chosenColor)
        {
            if (await IsKingMateByAsync<FigureType>(FigureType.King, chosenColor, currentBoard, gameId) &&
                await IsKingMateByAsync<FigureType>(FigureType.Queen, chosenColor, currentBoard, gameId) &&
                await IsKingMateByAsync<FigureType>(FigureType.Rook, chosenColor, currentBoard, gameId) &&
                await IsKingMateByAsync<FigureType>(FigureType.Knight, chosenColor, currentBoard, gameId) &&
                await IsKingMateByAsync<FigureType>(FigureType.Bishop, chosenColor, currentBoard, gameId) &&
                await IsKingMateByAsync<FigureType>(FigureType.Pawn, chosenColor, currentBoard, gameId))
                return await Task.FromResult(true);
            return await Task.FromResult(false);
        }

        private async Task<bool> IsKingMateByAsync<TFigureType>(TFigureType figureType, Turn myColor,
            Board? currentBoard, Guid gameId) where TFigureType : Enum
        {
            if (currentBoard == null)
                return await Task.FromResult(false);

            var figureBlocks =
                currentBoard.GetBlockByFigureTypeAndColor((FigureType)(object)figureType, (FigureColors)myColor);

            foreach (var figureBlock in figureBlocks)
            {
                if ((Turn)myColor != currentBoard?.Turn)
                    return await Task.FromResult(false);

                //Get all movable and cuttable blocks for the king
                //If there are any movable or cuttable blocks that do not result in a check, return true

                var figureMovableAndCuttable = figureBlock.Figure
                    .GetMovableAndCutableBlocks(figureBlock.Position, currentBoard);

                if (figureMovableAndCuttable is
                        not { MovableBlock: not null, CutableBlock: not null } ||
                    (!figureMovableAndCuttable.MovableBlock.Any() &&
                     !figureMovableAndCuttable.CutableBlock.Any()))
                    return await Task.FromResult(true);

                var cuttable = figureMovableAndCuttable.CutableBlock;
                var movable = figureMovableAndCuttable.MovableBlock;
                var executables = cuttable.Concat(movable);
                var enumerableOfExecutable = executables.ToList();

                if (enumerableOfExecutable.Any(executable =>
                        executable.EventColor is not EventColors.Cut and not EventColors.Move))
                    return await Task.FromResult(false);

                var submitMoveRequestDTO = new SubmitMoveRequestDTO()
                {
                    CurrentBoardState = currentBoard,
                    From = figureBlock.Position,
                    GameId = gameId
                };

                //Simulate each possible move for the king and check if it results in a check
                foreach (var executable in enumerableOfExecutable)
                {
                    submitMoveRequestDTO.To = executable.Position;
                        
                    var toBlockFigureTemp = currentBoard.GetBlockByPosition(executable.Position).Figure;

                    var submitMoveConnectionResponseDto = await SubmitMoveAsync(submitMoveRequestDTO);

                    if (submitMoveConnectionResponseDto is { Data.IsKingChecked: true })
                         continue;

                    //If the move does not result in a check, we need to reset the board state
                    ResetEventableBlocks(currentBoard);

                    //King Block is temporarily moved to executable position
                    //revert Move
                    //Get the moved king block

                    //Get the from block
                    var fromBlock =
                        currentBoard.GetBlockByPosition(figureBlock.Position);

                    var toBlock =
                        currentBoard.GetBlockByPosition(executable.Position);

                    var fromTempFigure = fromBlock.Figure;

                    //Revert the move
                    fromBlock.Figure = toBlock.Figure;
                    toBlock.Figure = toBlockFigureTemp;

                    return await Task.FromResult(false);

                }
                return await Task.FromResult(true);
            }


            return await Task.FromResult(false);
        }
    }
}
