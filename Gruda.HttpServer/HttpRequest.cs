using System.Text.Json;
using Gruda.HttpServer.Cookies;
using Gruda.HttpServer.Headers;
using Gruda.HttpServer.IdentityManagement;

namespace Gruda.HttpServer;

public sealed class HttpRequest
{
    public Identity? Identity { get; internal set; } = null!;

    public RequestCookies Cookies { get; private set; } = new();
    public HttpRequestHeaders Headers { get; } = new();

    public string Url { get; internal set; } = null!;

    public string Method { get; internal set; } = null!;

    public string ProtocolVersion { get; internal set; } = null!;

    public byte[]? BufferedBody { get; internal set; }

    public T? ReadBodyAsJson<T>() => BufferedBody is null ? default : JsonSerializer.Deserialize<T>(BufferedBody!);

    internal void AddHttpHeader(string key, string value)
    {
        Headers.Set(key, value);

        if (key == HttpHeaderConstants.Cookie)
        {
            Cookies.ParseCookies(value);
        }
    }
}