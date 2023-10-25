using Gruda.HttpServer.Middleware;

namespace Gruda.HttpServer.Program;

public sealed class TestMiddleware : IHttpMiddleware
{
    public async Task InvokeAsync(HttpContext context, Func<HttpContext, Task> next)
    {
        // Console.WriteLine("TestMiddleware: " + context.Request.Identity?.Name);
        await next(context);
    }
}