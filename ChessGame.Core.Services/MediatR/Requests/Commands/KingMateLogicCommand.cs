using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Commands
{
    public class KingMateLogicCommand<TRequest,TResponse> : IRequest<TResponse>
    {
        public TRequest RequestDTO { get; set; }
        public KingMateLogicCommand(TRequest requestDTO)
        {
            RequestDTO = requestDTO;
        }
    }
}
