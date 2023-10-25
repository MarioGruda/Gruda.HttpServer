using Gruda.HttpServer.Headers;

namespace Gruda.HttpServer.Cookies;

// TODO: Cookie handling
public sealed class ResponseCookies : Cookies
{
    private readonly HttpResponseHeaders _headers;

    public ResponseCookies(HttpResponseHeaders headers)
    {
        _headers = headers;
    }

    internal void Append(string key, string value)
    {
        CookiesStore.Add(key, value);
        _headers.SetCookie = (_headers.SetCookie ?? "") + $"{key}={value};";
    }

    internal void Remove(string key)
    {
        CookiesStore.Remove(key);
        _headers.SetCookie = (_headers.SetCookie ?? "") + $"{key}=; Expires=Thu, 01 Jan 1970 00:00:00 GMT;";
    }
}