using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Commands
{
    public class BoardInitializeCommand<TRequest,TResponse> : IRequest<TResponse>
    {
        public TRequest RequestDTO { get; set; }
        public BoardInitializeCommand(TRequest request)
        {
            RequestDTO  = request;
        }
    }
}
