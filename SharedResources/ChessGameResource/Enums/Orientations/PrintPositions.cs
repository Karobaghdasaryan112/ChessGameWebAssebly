namespace SharedResources.ChessGameResource.Enums.Orientations;

public class PrintPositions
{
    public static string ToChessPosition(int column, int row)
    {
        char file = (char)('a' + column - 1);
        return $"{file}{row}";
    }
}