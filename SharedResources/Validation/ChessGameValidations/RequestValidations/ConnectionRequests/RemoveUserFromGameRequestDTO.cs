using SharedResources.Contracts;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests
{
    public class RemoveUserFromGameRequestDTO : IRequestDTO
    {
        public Guid GameId { get; set; }
    }
}
