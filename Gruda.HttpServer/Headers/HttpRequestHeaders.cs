namespace Gruda.HttpServer.Headers;

public sealed class HttpRequestHeaders : HttpHeaders
{
    public string? Accept => this["Accept"];

    public string? AcceptCharset => this["Accept-Charset"];

    public string? AcceptEncoding => this["Accept-Encoding"];

    public string? Connection => this["Connection"];

    public long ContentLength => long.Parse(this["Content-Length"]);

    public string? ContentType => this["Content-Type"];

    public string? Host => this["Host"];
}