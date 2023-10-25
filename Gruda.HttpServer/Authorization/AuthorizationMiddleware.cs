using Gruda.HttpServer.Middleware;

namespace Gruda.HttpServer.Authorization;

public class AuthorizationMiddleware : IHttpMiddleware
{
    public async Task InvokeAsync(HttpContext context, Func<HttpContext, Task> next)
    {
        if (!context.TryGetMetadata<AuthorizationHandler>(out var handler))
        {
            await next(context);
            return;
        }

        AuthorizationResult result = await handler.HandleAsync(context).ConfigureAwait(false);

        if (result == AuthorizationResult.Failure)
        {
            await context.Response.Unauthorized().ConfigureAwait(false);
            return;
        }

        await next(context);
    }
}