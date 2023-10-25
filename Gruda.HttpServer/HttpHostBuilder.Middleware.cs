using Gruda.HttpServer.Middleware;

namespace Gruda.HttpServer;

public partial class HttpHostBuilder
{
    private readonly List<MiddlewareMetadata> _middlewareMetadata = new();

    public HttpHostBuilder Use<TMiddleware>() where TMiddleware : class, IHttpMiddleware, new()
    {
        TMiddleware middlewareInstance = new();
        _middlewareMetadata.Add(new MiddlewareMetadata(middlewareInstance));
        return this;
    }

    public HttpHostBuilder Use(Func<HttpContext, Func<HttpContext, Task>, Task> middleware)
    {
        IHttpMiddleware middlewareInstance = new DelegateMiddleware(middleware);
        _middlewareMetadata.Add(new MiddlewareMetadata(middlewareInstance));
        return this;
    }

    private MiddlewareMetadata? BuildMiddlewarePipeline()
    {
        if (_middlewareMetadata.Count == 0)
            return null;

        // last middleware in the pipeline is the one that will process the request via "next"
        _middlewareMetadata[^1].Next = (context) => context.Endpoint.RouteDelegate(context);

        // build the pipeline from the last middleware to the first
        if (_middlewareMetadata.Count > 1)
        {
            for (int index = _middlewareMetadata.Count - 1; index > 0; index--)
            {
                var middleWare = _middlewareMetadata[index].Middleware;
                var next = _middlewareMetadata[index].Next;

                var prevMiddleware = _middlewareMetadata[index - 1];
                prevMiddleware.Next = ctx => middleWare.InvokeAsync(ctx, next);
            }
        }

        return _middlewareMetadata[0];
    }
}