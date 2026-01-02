using SharedResources.ChessGameResource.Enums.FigureTypes;

namespace SharedResources.ChessGameResource.Enums.Scores
{
    public static class FigureScores
    {
        private static readonly Dictionary<GamePhase, Dictionary<FigureType, int>> Scores =
            new Dictionary<GamePhase, Dictionary<FigureType, int>>
        {
            {
                GamePhase.StartGame, new Dictionary<FigureType, int>
                {
                    { FigureType.Pawn, 10 },
                    { FigureType.Knight, 32 },
                    { FigureType.Bishop, 33 },
                    { FigureType.Rook, 50 },
                    { FigureType.Queen, 90 },
                    { FigureType.King, 500 }
                }
            },
            {
                GamePhase.Midgame, new Dictionary<FigureType, int>
                {
                    { FigureType.Pawn, 12 },
                    { FigureType.Knight, 30 },
                    { FigureType.Bishop, 33 },
                    { FigureType.Rook, 52 },
                    { FigureType.Queen, 95 },
                    { FigureType.King, 500 }
                }
            },
            {
                GamePhase.Endgame, new Dictionary<FigureType, int>
                {
                    { FigureType.Pawn, 25 },
                    { FigureType.Knight, 25 },
                    { FigureType.Bishop, 35 },
                    { FigureType.Rook, 55 },
                    { FigureType.Queen, 90 },
                    { FigureType.King, 500 }
                }
            }
        };


        public static int GetFigureScore(GamePhase gamePhase, FigureType figureType)
        {
            if (Scores.TryGetValue(gamePhase, out var phaseScores) && phaseScores.TryGetValue(figureType, out var score))
            {
                return score;
            }

            return 0;
        }
    }
}
