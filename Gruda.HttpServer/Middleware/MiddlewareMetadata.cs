namespace Gruda.HttpServer.Middleware;

internal class MiddlewareMetadata
{
    public IHttpMiddleware Middleware { get; init; }
    public Func<HttpContext, Task> Next { get; internal set; } = null!;

    public MiddlewareMetadata(IHttpMiddleware middleware)
    {
        Middleware = middleware;
    }

    public void Deconstruct(out IHttpMiddleware middleware, out Func<HttpContext, Task> next)
    {
        middleware = Middleware;
        next = Next;
    }
}