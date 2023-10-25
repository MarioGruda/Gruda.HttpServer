namespace Gruda.HttpServer;

public partial class HttpHost
{
    private readonly Dictionary<(string Url, string HttpMethod), Endpoint> _routes = new();

    private Endpoint? _fallbackHandler;

    public Endpoint MapGet(string url, Func<HttpContext, Task> handler) => Map(url, "GET", handler);


    public Endpoint MapPost(string url, Func<HttpContext, Task> handler) => Map(url, "POST", handler);

    public Endpoint MapPut(string url, Func<HttpContext, Task> handler) => Map(url, "PUT", handler);

    public Endpoint MapDelete(string url, Func<HttpContext, Task> handler) => Map(url, "DELETE", handler);

    public Endpoint MapGetFallback(Func<HttpContext, Task> handler) => Map("*", "GET", handler);

    public Endpoint MapFallback(Func<HttpContext, Task> handler)
    {
        _fallbackHandler = new Endpoint(handler);
        return _fallbackHandler;
    }

    private Endpoint Map(string url, string method, Func<HttpContext, Task> handler)
    {
        Endpoint endpoint = new(handler);
        _routes.Add((url, method), endpoint);
        return endpoint;
    }
}