using Gruda.HttpServer.Middleware;

namespace Gruda.HttpServer.IdentityManagement;

public class CookieAuthenticationMiddleware : IHttpMiddleware
{
    public async Task InvokeAsync(HttpContext context, Func<HttpContext, Task> next)
    {
        context.Request.Identity = IdentityManager.GetIdentity(context);
        await next(context);
    }
}