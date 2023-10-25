namespace Gruda.HttpServer.Middleware;

public interface IHttpMiddleware
{
    Task InvokeAsync(HttpContext context, Func<HttpContext, Task> next);
}