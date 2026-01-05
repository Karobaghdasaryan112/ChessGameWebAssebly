using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Commands
{
    public class AIMoveLogicCommand<TRequest, TResponse> : IRequest<TResponse>
    {
        public TRequest RequestDTO { get; set; }
        public AIMoveLogicCommand(TRequest requestDTO)
        {
            RequestDTO = requestDTO;
        }
    }
}
