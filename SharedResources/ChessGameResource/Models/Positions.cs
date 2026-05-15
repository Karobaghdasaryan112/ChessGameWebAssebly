
using SharedResources.ChessGameResource.Enums.Orientations;

namespace SharedResources.ChessGameResource.Models;

public class Position
{
    public VerticalOrientation VerticalOrientation { get; set; }
    public HorizontalOrientation HorizontalOrientation { get; set; }
    public Position() { }
    public Position(
        VerticalOrientation verticalOrientation,
        HorizontalOrientation horizontalOrientation)
    {
        VerticalOrientation = verticalOrientation;
        HorizontalOrientation = horizontalOrientation;
    }

    public Position(int verticalOrientation, int horizontalOrientation)
    {
        VerticalOrientation = (VerticalOrientation)verticalOrientation;
        HorizontalOrientation = (HorizontalOrientation)horizontalOrientation;
    }
    public override bool Equals(object? obj)
    {
        if (obj is not Position) return false;
        if(obj is null) return false;
        var position = obj as Position;

        return position!.VerticalOrientation == this.VerticalOrientation &&
            position.HorizontalOrientation == this.HorizontalOrientation;
    }
    public override string ToString()
        => PrintPositions.ToChessPosition((int)VerticalOrientation, (int)HorizontalOrientation);
}
