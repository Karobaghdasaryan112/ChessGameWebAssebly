using BlazorServerSideClient.Contracts.Handlers;
using Microsoft.AspNetCore.Components;

namespace BlazorServerSideClient.Services.Handlers
{
    public class HistoryWidgetHandlerService(JSRunetimeService JSRunetimeService,NavigationManager navigationManager) : IHistoryWidgetHandlerService
    {
    }
}
