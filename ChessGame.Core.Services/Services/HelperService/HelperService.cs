using ChessGame.Core.Services.Extentions;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Enums.Scores;
using SharedResources.ChessGameResource.Figures;
using SharedResources.ChessGameResource.Models;

namespace ChessGame.Core.Services.Services.HelperService
{
    public class HelperService
    {
        public bool IsKingMateStateByHelper(Board board, FigureColors chosenColor)
        {
            if (IsKingMateByAsync<FigureType>(FigureType.King, (Turn)chosenColor, board) &&
                IsKingMateByAsync<FigureType>(FigureType.Queen, (Turn)chosenColor, board) &&
                IsKingMateByAsync<FigureType>(FigureType.Rook, (Turn)chosenColor, board) &&
                IsKingMateByAsync<FigureType>(FigureType.Knight, (Turn)chosenColor, board) &&
                IsKingMateByAsync<FigureType>(FigureType.Bishop, (Turn)chosenColor, board) &&
                IsKingMateByAsync<FigureType>(FigureType.Pawn, (Turn)chosenColor, board))
                return true;
            return false;
        }

        private bool IsKingMateByAsync<TFigureType>(TFigureType figureType, Turn myColor,
            Board? currentBoard) where TFigureType : Enum
        {
            if (currentBoard == null)
                return false;
            var figureBlocks =
                currentBoard.GetBlockByFigureTypeAndColor((FigureType)(object)figureType, (FigureColors)myColor);
            if (!figureBlocks.Any())
                return true;
            foreach (var figureBlock in figureBlocks)
            {
                if ((Turn)myColor != currentBoard?.Turn)
                    return false;

                var figureMovableAndCuttable = figureBlock.Figure
                    .GetMovableAndCuttableBlocks(figureBlock.Position, currentBoard);
                if (figureMovableAndCuttable is
                        not { MovableBlock: not null, CutableBlock: not null } ||
                    (!figureMovableAndCuttable.MovableBlock.Any() &&
                     !figureMovableAndCuttable.CutableBlock.Any()))
                    return true;

                var cuttable = figureMovableAndCuttable.CutableBlock;
                var movable = figureMovableAndCuttable.MovableBlock;

                var executables = cuttable.Concat(movable);

                var enumerableOfExecutable = executables.ToList();

                if (enumerableOfExecutable.Any(executable =>
                        executable.EventColor is not EventColors.Cut and not EventColors.Move))
                    return false;



                foreach (var executable in enumerableOfExecutable)
                {
                    var toBlockFigureTemp = currentBoard.GetBlockByPosition(executable.Position).Figure;


                    var submitMoveCommand = SubmitMoveByHelper(figureBlock.Position, executable.Position, currentBoard);


                    if (submitMoveCommand is { IsKingChecked: true })
                        continue;

                    currentBoard.ResetEventableBlocks();

                    var fromBlock =
                        currentBoard.GetBlockByPosition(figureBlock.Position);

                    var toBlock =
                        currentBoard.GetBlockByPosition(executable.Position);
                    var fromTempFigure = fromBlock.Figure;

                    fromBlock.Figure = toBlock.Figure;
                    toBlock.Figure = toBlockFigureTemp;

                    return false;
                }

                return true;
            }

            return false;
        }

        public bool IsKingCheckByHelper(FigureColors chosenColor, Board currentBoard)
        {
            var myColor = (FigureColors)chosenColor;
            if (currentBoard == default || chosenColor == FigureColors.None)
            {
                var x = 10;
            }

            var kingBlock = currentBoard.GetBlockByFigureTypeAndColor(FigureType.King, myColor).First();

            if (IsKingCheckedBy<FigureType>(kingBlock, FigureType.Queen, myColor, currentBoard,
                    [FigureType.Queen]) ||
                IsKingCheckedBy<FigureType>(kingBlock, FigureType.Rook, myColor, currentBoard,
                    [FigureType.Rook, FigureType.Queen]) ||
                IsKingCheckedBy<FigureType>(kingBlock, FigureType.Bishop, myColor, currentBoard,
                    [FigureType.Queen, FigureType.Bishop]) ||
                IsKingCheckedBy<FigureType>(kingBlock, FigureType.Knight, myColor, currentBoard,
                    [FigureType.Knight]) ||
                IsKingCheckedBy<FigureType>(kingBlock, FigureType.King, myColor, currentBoard,
                    [FigureType.King]) ||
                IsKingCheckedBy<FigureType>(kingBlock, FigureType.Pawn, myColor, currentBoard,
                    [FigureType.Pawn, FigureType.Bishop, FigureType.King, FigureType.Queen]))
                return true;
            return false;
        }

        private bool IsKingCheckedBy<TFigureType>(Block kingBlock, TFigureType figureType,
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
                kingBlockClone.Figure.GetMovableAndCuttableBlocks(kingBlockClone.Position, currentBoard,
                    kingBlockClone);

            var figuresForCheck = possibleMovableAndCuttable.CutableBlock.Where(block =>
                figureTypes.Contains<FigureType>(block.Figure.FigureType));

            var figureForChecks = figuresForCheck as Block[] ?? figuresForCheck.ToArray();

            if (!figureForChecks.Any())
                return false;

            return true;
        }


        public SubmitMoveByHelperDTO SubmitMoveByHelper(Position from, Position to, Board boardState)
        {
            var response =
                new SubmitMoveByHelperDTO()
                {
                    IsKingChecked = false,
                    IsKingMate = false,
                    IsMoveSuccess = true
                };


            var fromBlock = boardState.GetBlockByPosition(from!);
            var toBlock = boardState.GetBlockByPosition(to!);

            boardState.ResetEventableBlocks();

            if (fromBlock?.Figure == null)
            {
                response.IsMoveSuccess = false;
                return response;
            }
            if (toBlock?.Figure?.FigureType == FigureType.King)
            {
                response.IsMoveSuccess = false;
                return response;
            }
            var toBlockTemp = toBlock.Figure;

            toBlock.Figure = fromBlock.Figure;
            fromBlock.Figure = null!;

            var isKingCheckResponse = IsKingCheckByHelper((FigureColors)boardState.Turn, boardState);

            if (!isKingCheckResponse)
            {
                boardState.SwitchTurn();
                return response;
            }

            fromBlock.Figure = toBlock.Figure;
            toBlock.Figure = toBlockTemp;
            response.IsKingChecked = true;
            response.IsMoveSuccess = false;
            return response;
        }

        public GamePhase GetGamePhase(Board board)
        {
            int materialSum = 0;

            foreach (var block in board.BoardBlocks.SelectMany(x => x).Where(b => b.Figure != null))
            {
                switch (block.Figure.FigureType)
                {
                    case FigureType.Pawn: materialSum += 1; break;
                    case FigureType.Knight: materialSum += 3; break;
                    case FigureType.Bishop: materialSum += 3; break;
                    case FigureType.Rook: materialSum += 5; break;
                    case FigureType.Queen: materialSum += 9; break;
                }
            }

            double materialPercent = materialSum / 39.0 * 100;

            if (materialPercent > 70)
                return GamePhase.StartGame;
            else if (materialPercent > 30)
                return GamePhase.Midgame;
            else
                return GamePhase.Endgame;
        }
    }


    public class SubmitMoveByHelperDTO
    {
        public bool IsKingChecked { get; set; }
        public bool IsKingMate { get; set; }
        public bool IsMoveSuccess { get; set; }
    }

}
