using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;

namespace SharedResources.ChessGameResource.Models
{
    public class Blocks
    {
        public Position Position { get; set; }
        public Figures Figures { get; set; }
        public FigureColors figureColor { get; set; }
    }
}
