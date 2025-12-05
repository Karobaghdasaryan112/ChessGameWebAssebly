using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Queries
{
    public class GetMoveCommand<TRequest, Tresponse>(TRequest request) : IRequest<Tresponse>
    {
        public TRequest RequestDTO { get; set; } = request;
    }
}
