using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Queries
{
    public class GetGameHistoryQuery<TRequest, TResponse> : IRequest<TResponse>
    {
        public TRequest RequestType { get; set; }
        public GetGameHistoryQuery(TRequest request)
        {
            RequestType = request;
        }
    }
}
