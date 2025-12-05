using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Queries
{
    public class GetHistoryWidgetsPaginationQuery<TRequest, TResponse>(TRequest request) : IRequest<TResponse>
    {
        public TRequest Request { get; set; } = request;
    }
}
