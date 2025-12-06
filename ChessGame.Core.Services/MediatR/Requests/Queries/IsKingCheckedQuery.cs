using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Queries
{
    public class IsKingCheckedQuery<TRequest, TResponse>(TRequest request) : IRequest<TResponse>
    {
        public TRequest Request = request;
    }
}
