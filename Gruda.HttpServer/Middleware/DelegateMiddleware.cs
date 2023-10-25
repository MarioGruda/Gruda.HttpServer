namespace Gruda.HttpServer.Middleware;

internal class DelegateMiddleware : IHttpMiddleware
{
    private Func<HttpContext, Func<HttpContext, Task>, Task> InvokeAsyncDelegate { get; }

    public DelegateMiddleware(Func<HttpContext, Func<HttpContext, Task>, Task> invokeAsyncDelegate)
    {
        InvokeAsyncDelegate = invokeAsyncDelegate;
    }

    public async Task InvokeAsync(HttpContext context, Func<HttpContext, Task> next) => await InvokeAsyncDelegate(context, next).ConfigureAwait(false);
}