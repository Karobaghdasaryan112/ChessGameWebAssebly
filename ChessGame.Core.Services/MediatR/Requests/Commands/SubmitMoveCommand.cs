using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Commands
{
    public class SubmitMoveCommand<TRequest, TResponse> : IRequest<TResponse>
    {
        public TRequest RequestDTO { get; set; }
        public SubmitMoveCommand(TRequest request)
        {
            RequestDTO = request;
        }
    }
}
