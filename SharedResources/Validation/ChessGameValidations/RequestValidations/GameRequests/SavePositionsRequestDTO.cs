using SharedResources.ChessGameResource.Models;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class SavePositionsRequestDTO
    {
        public Guid GameId { get; set; }
        public string FEN { get; set; }

    }
}
