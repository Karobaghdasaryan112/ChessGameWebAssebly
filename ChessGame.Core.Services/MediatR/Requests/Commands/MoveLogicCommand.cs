using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Commands
{
    public class MoveLogicCommand<TRequest, TResponse>(TRequest request) : IRequest<TResponse>
    {
        public TRequest Request { get; set; } = request;
    }
}
