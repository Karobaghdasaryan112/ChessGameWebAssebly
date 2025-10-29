using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;

namespace IdentityServer.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using OpenIddict.Server;
    using OpenIddict.Server.AspNetCore;

    namespace IdentityServer.Controllers
    {
        //[ApiController]
        //[Route("connect")]
        //public class AuthorizationController : Controller
        //{
        //    private readonly IOpenIddictServerDispatcher _dispatcher;

        //    public AuthorizationController(IOpenIddictServerDispatcher dispatcher)
        //    {
        //        _dispatcher = dispatcher;
        //    }

        //    [HttpGet("authorize")]
        //    [HttpPost("authorize")]
        //    public async Task<IActionResult> Authorize()
        //    {
        //        // Create an OpenIddict ASP.NET Core context
        //        var context = new OpenIddictServerAspNetCoreRequestContext(HttpContext);

        //        // Dispatch the request to OpenIddict
        //        await _dispatcher.DispatchAsync(context);

        //        // The response is now available in context.Response
        //        if (context.Response != null)
        //        {
        //            return context.Response switch
        //            {
        //                IActionResult actionResult => actionResult,
        //                _ => throw new InvalidOperationException("Unexpected response type")
        //            };
        //        }

        //        return BadRequest();
        //    }
        //}
    }


}
