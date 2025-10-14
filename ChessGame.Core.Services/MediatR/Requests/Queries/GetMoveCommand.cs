using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Queries
{
    public class GetMoveCommand<TRequest, Tresponse> : IRequest<Tresponse>
    {
        public TRequest RequestDTO { get; set; }
        public GetMoveCommand(TRequest request)
        {
            RequestDTO = request;
        }
    }
}
