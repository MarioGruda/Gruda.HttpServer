using System.Net;
using Gruda.HttpServer.Middleware;

namespace Gruda.HttpServer;

public partial class HttpHostBuilder
{
    private readonly int _port;
    private readonly IPAddress _address;

    public HttpHostBuilder(IPAddress address, int port)
    {
        _address = address;
        _port = port;
    }

    public HttpHost Build()
    {
        MiddlewareMetadata? firstMiddleware = BuildMiddlewarePipeline();
        return new HttpHost(_address, _port, firstMiddleware);
    }
}