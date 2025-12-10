using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Queries
{
    public class SendClickQuery<TRequest, TResponse> : IRequest<TResponse>
    {
        public TRequest Request { get; set; }
        public SendClickQuery(TRequest request) => Request = request;
    }
}
