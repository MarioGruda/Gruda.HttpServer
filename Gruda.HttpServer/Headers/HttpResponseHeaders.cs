namespace Gruda.HttpServer.Headers;

public sealed class HttpResponseHeaders : HttpHeaders
{
    public long ContentLength
    {
        get
        {
            if (Headers.TryGetValue(HttpHeaderConstants.ContentLength, out var value))
                return long.Parse(value);

            return 0;
        }
        set => this["Content-Length"] = value.ToString();
    }
    
    public DateTimeOffset? Date
    {
        get
        {
            if (Headers.TryGetValue("Date", out var value))
                return DateTimeOffset.Parse(value);

            return null;
        }
        set => this["Date"] = value.ToString();
    }

    public string? ContentType
    {
        get => this["Content-Type"];
        set => this["Content-Type"] = value;
    }

    public string? Host
    {
        get => this["Host"];
        set => this["Host"] = value;
    }

    public string? SetCookie
    {
        get => this["Set-Cookie"];
        set => this["Set-Cookie"] = value;
    }
}