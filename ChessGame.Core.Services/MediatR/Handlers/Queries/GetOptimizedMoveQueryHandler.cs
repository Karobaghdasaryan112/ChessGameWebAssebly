using ChessGame.Core.Services.Constants;
using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Core.Services.Services.HelperService;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.Scores;
using SharedResources.ChessGameResource.Models;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;
using System.Net;


namespace ChessGame.Core.Services.MediatR.Handlers.Queries
{
    public class GetOptimizedMoveQueryHandler(
        IValidator<GetOptimizedMoveRequestDTO> validator,
        ILogger<GetOptimizedMoveQueryHandler> logger,
        IBoardService boardService,
        HelperService helperService,
        GenericValidationService genericValidationService) :
        MediatR_Base<GetOptimizedMoveRequestDTO, GetOptimizedMoveQueryHandler, IBoardService>(validator, logger,
            boardService),
        IRequestHandler<
            GetOptimizedMoveQuery<
                GetOptimizedMoveRequestDTO,
                ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>>,
            ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>>
    {

        public class SearchResult
        {
            public double Score { get; set; }
            public Position? From { get; set; }
            public Position? To { get; set; }
        }

        public async Task<ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>>
            Handle(
                GetOptimizedMoveQuery<GetOptimizedMoveRequestDTO,
                    ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>> request,
                CancellationToken cancellationToken)
        {
            //Valdidation
            var validationResult = await genericValidationService.ValidateAsync(request.RequestDTO);
            if (!validationResult.IsValid)
            {
                return await Task.FromResult(ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>
                    .CreateErrorResponse(ChessGameResponseMessage.InvalidData, System.Net.HttpStatusCode.BadRequest,
                        validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
            }

            //Existing Game Check
            var board = ActiveGames.GetBoard(request.RequestDTO.GameId);
            if (board == null)
            {
                return await Task.FromResult(ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>
                    .CreateErrorResponse(ChessGameResponseMessage.GameNotFound, System.Net.HttpStatusCode.NotFound));
            }


            var aiColor = request.RequestDTO.ChosenColor;

            var result = AlphaBetaRoot(request.RequestDTO.GameId, board, HelperConstants.MAX_DEPTH, int.MinValue, int.MaxValue, aiColor);


            return ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(new GetOptimizedMoveResponseDTO
                {
                    FromPosition = result.From!,
                    ToPosition = result.To!,
                    GameId = request.RequestDTO.GameId
                },
                    ChessGameResponseMessage.SuccessData,
                    HttpStatusCode.OK);
        }

        private SearchResult AlphaBetaRoot(Guid gameId, Board board, int depth, double alpha, double beta, FigureColors aiColor)
        {
            bool isMaxRoot = (FigureColors)board.Turn == aiColor;

            SearchResult bestResult = new()
            {
                Score = isMaxRoot ? double.NegativeInfinity
                                  : double.PositiveInfinity
            };



            var moves = GeneratePossibleMoves(board);

            foreach (var move in moves)
            {
                foreach (var to in move.Value)
                {
                    var nextBoard = (Board)board.Clone();

                    var submit = TryApplyMove(gameId, nextBoard, move.Key, to);
                    if (!submit)
                        continue;

                    var score = AlphaBeta(gameId, nextBoard, depth - 1, alpha, beta, aiColor);

                    if (isMaxRoot)
                    {
                        if (score > bestResult.Score)
                        {
                            bestResult.Score = score;
                            bestResult.From = move.Key;
                            bestResult.To = to;
                        }
                        alpha = Math.Max(alpha, score);
                    }
                    else
                    {
                        if (score < bestResult.Score)
                        {
                            bestResult.Score = score;
                            bestResult.From = move.Key;
                            bestResult.To = to;
                        }
                        beta = Math.Min(beta, score);
                    }

                    if (beta <= alpha)
                        break;
                }
            }

            return bestResult;
        }


        private double AlphaBeta(Guid gameId, Board board, int depth, double alpha, double beta, FigureColors aiColor)
        {
            if (depth == 0)
                return Evaluate(gameId, board, aiColor);

            bool isMaximizing = (FigureColors)board.Turn == aiColor;

            var moves = GeneratePossibleMoves(board);

            if (isMaximizing)
            {
                double maxEval = int.MinValue;

                foreach (var move in moves)
                {
                    foreach (var to in move.Value)
                    {
                        var nextBoard = (Board)board.Clone();

                        if (!TryApplyMove(gameId, nextBoard, move.Key, to))
                            continue;


                        var eval = AlphaBeta(gameId, nextBoard, depth - 1, alpha, beta, aiColor);

                        maxEval = Math.Max(maxEval, eval);
                        alpha = Math.Max(alpha, eval);

                        if (beta <= alpha)
                            return maxEval;
                    }
                }

                return maxEval;
            }
            else
            {
                double minEval = int.MaxValue;

                foreach (var move in moves)
                {
                    foreach (var to in move.Value)
                    {
                        var nextBoard = (Board)board.Clone();

                        if (!TryApplyMove(gameId, nextBoard, move.Key, to))
                            continue;

                        double eval = AlphaBeta(gameId, nextBoard, depth - 1, alpha, beta, aiColor);

                        minEval = Math.Min(minEval, eval);
                        beta = Math.Min(beta, eval);

                        if (beta <= alpha)
                            return minEval;
                    }
                }

                return minEval;
            }
        }

        private bool TryApplyMove(Guid gameId, Board board, Position from, Position to)
        {
            var result = helperService.SubmitMoveByHelper(from, to, board);

            return result is { IsMoveSuccess: true };
        }

        private double Evaluate(
            Guid gameId,
            Board board,
            FigureColors aiColor)
        {
            double score = 0;

            var gamePhase = helperService.GetGamePhase(board);

            var blocks = board.BoardBlocks
                .SelectMany(x => x)
                .Where(b => b.Figure != null)
                .ToList();

            score += MaterialScore(blocks, board, gamePhase, aiColor);
            score += KingSafetyScore(gameId, board, aiColor);
            return score;
        }


        private double MaterialScore(List<Block> blocks, Board board, GamePhase gamePhase, FigureColors aiColor)
        {
            double score = 0;

            foreach (var block in blocks)
            {   
                double value = FigureScores.GetFigureScore(gamePhase, block.Figure.FigureType);
                value += block.Figure.GetPositionalScore(
                            block.Position,
                            gamePhase,
                            block.Figure.FigureColor == FigureColors.White
                        );
                score += block.Figure.FigureColor == aiColor ? value : -value;
            }

            return score;
        }

        private int KingSafetyScore(
            Guid gameId,
            Board board,
            FigureColors aiColor)
        {

            int score = 0;

            if (helperService.IsKingMateStateByHelper(board, aiColor))
                return -100_000;

            var opponent = aiColor == FigureColors.White
                ? FigureColors.Black
                : FigureColors.White;

            if (helperService.IsKingMateStateByHelper(board, opponent))
                return 100_000;

            return score;

        }


        public List<KeyValuePair<Position, List<Position>>> GeneratePossibleMoves(Board board)
        {
            var possibleMoves = new List<KeyValuePair<Position, List<Position>>>();
            foreach (var block in board.BoardBlocks.SelectMany(blocks =>
                         blocks.Where(block =>
                             block.Figure != null && block.Figure?.FigureColor == (FigureColors)board.Turn)))
            {

                var moves = block.Figure.GetMovableAndCuttableBlocks(block.Position, board);

                var movables = moves.MovableBlock.Select(block => block.Position).ToList();
                var executables = moves.CutableBlock.Select(block => block.Position).ToList();
                executables.AddRange(movables);
                possibleMoves.Add(new KeyValuePair<Position, List<Position>>(block.Position, executables));
            }

            return possibleMoves;
        }
    }
}
