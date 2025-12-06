using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace ChessGame.Core.Services.MediatR.Requests.Queries
{
    public class IsKingMateQuery<TRequest, TResponse>(TRequest request) : IRequest<TResponse>
    {
        public TRequest Request { get; set; } = request;
    }
}
