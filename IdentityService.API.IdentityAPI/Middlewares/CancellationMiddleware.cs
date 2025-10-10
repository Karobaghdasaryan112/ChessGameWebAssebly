namespace IdentityService.API.IdentityAPI.Middlewares
{
    public class CancellationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CancellationMiddleware> _logger;
        public CancellationMiddleware(RequestDelegate next, ILogger<CancellationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.RequestAborted;

            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                _logger.LogWarning("Request was canceled by the client.");
                context.Response.StatusCode = 499;
            }
        }
    }
}
