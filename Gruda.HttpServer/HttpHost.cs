using System.IO.Pipelines;
using System.Net;
using System.Threading.Channels;
using Gruda.HttpServer.Middleware;

namespace Gruda.HttpServer;

public partial class HttpHost
{
    public int Port { get; init; }
    public IPAddress Address { get; init; }

    private readonly MiddlewareMetadata? _firstMiddleWare;

    private HttpServer _server = null!;

    private CancellationToken _cancellationToken;


    internal HttpHost(IPAddress address, int port, MiddlewareMetadata? firstMiddleWare)
    {
        Address = address;
        Port = port;
        _firstMiddleWare = firstMiddleWare;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _cancellationToken = cancellationToken;

        _server = new HttpServer(Address, Port);
        _cancellationToken.Register(() => _server.Stop());

        return _server.StartAsync(Run, _cancellationToken);
    }

    public Task StopAsync()
        => _server.Stop();

    private async Task Run(HttpContext ctx)
    {
        try
        {
            await RunRequestPipeline(ctx).ConfigureAwait(false);

            if (!ctx.Response.Started)
                await ctx.Response.Ok().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ctx?.Response is not null)
                await ctx.Response.InternalServerError(ex.Message).ConfigureAwait(false);
        }
        finally
        {
            PipeWriter? writer = ctx?.Response?.BodyWriter;

            if (writer is not null)
            {
                await writer.FlushAsync(_cancellationToken).ConfigureAwait(false);
                //   await writer.CompleteAsync().ConfigureAwait(false);
            }
        }
    }

    private async Task RunRequestPipeline(HttpContext context)
    {
        Endpoint? endpoint = IdentifyEndpoint(context);

        if (endpoint is null)
        {
            await context.Response.NotFound().ConfigureAwait(false);
            return;
        }

        // attach endpoint to the context, so that middleware can access it`s route delegate and metadata
        context.Endpoint = endpoint;

        if (_firstMiddleWare is not null)
        {
            // first middleware in the pipeline invokes the next middleware
            (IHttpMiddleware firstMiddleware, Func<HttpContext, Task> next) = _firstMiddleWare;
            await firstMiddleware.InvokeAsync(context, next).ConfigureAwait(false);
            return;
        }

        // no middleware in the pipeline, invoke the endpoint directly
        await context.Endpoint.RouteDelegate(context).ConfigureAwait(false);
    }

    private Endpoint? IdentifyEndpoint(HttpContext context)
    {
        if (!_routes.TryGetValue((context.Request.Url, context.Request.Method), out var handler))
        {
            if (!_routes.TryGetValue(("*", context.Request.Method), out handler))
            {
                return _fallbackHandler;
            }
        }

        return handler;
    }
}