using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Queries
{
    public class GetOptimizedMoveQuery<TRequest, TResponse> : IRequest<TResponse>
    {
        public TRequest RequestDTO { get; set; }
        public GetOptimizedMoveQuery(TRequest request)
        {
            RequestDTO = request;
        }
    }
}
