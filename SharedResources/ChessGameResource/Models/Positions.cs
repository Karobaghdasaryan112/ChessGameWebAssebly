
using SharedResources.ChessGameResource.Enums.Orientations;

namespace SharedResources.ChessGameResource.Models;

public class Position
{
    public VerticalOrientation VerticalOrientation { get; set; }
    public HorizontalOrientation HorizontalOrientation { get; set; }

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
}
