using ChessGame.Core.Services.Contracts.BoardServices;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;

namespace ChessGame.Infrastructure.HubConnections
{
    public class SignalR_Utility : IHub
    {
        private readonly IBoardService BoardService;
        public SignalR_Utility(IBoardService  boardService)
        {
            BoardService = boardService;
        }

        public HubCallerContext Context { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IHubCallerConnectionContext<dynamic> Clients { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGroupManager Groups { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task OnConnected()
        {
            throw new NotImplementedException();
        }

        public Task OnDisconnected(bool stopCalled)
        {
            throw new NotImplementedException();
        }

        public Task OnReconnected()
        {
            throw new NotImplementedException();
        } 
    }
}
