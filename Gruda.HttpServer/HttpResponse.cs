using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using Gruda.HttpServer.Cookies;
using Gruda.HttpServer.Headers;

namespace Gruda.HttpServer;

public sealed class HttpResponse
{
    public bool Started { get; private set; } = false;
    public HttpResponseHeaders Headers { get; init; }
    public ResponseCookies? Cookies { get; private set; }
    public int? StatusCode { get; set; } = null;

    public string ProtocolVersion { get; }

    public PipeWriter BodyWriter { get; }

    public HttpResponse(PipeWriter bodyWriter, string protocolVersion)
    {
        Headers = new();
        BodyWriter = bodyWriter;
        ProtocolVersion = protocolVersion;
    }

    public void SetHttpHeader(string key, string value)
    {
        Headers.Set(key, value);
    }

    public void AppendCookie(string key, string value)
    {
        Cookies ??= new ResponseCookies(Headers);
        Cookies.Append(key, value);
    }

    public void RemoveCookie(string key)
    {
        Cookies ??= new ResponseCookies(Headers);
        Cookies.Remove(key);
    }

    public async Task NotFound()
    {
        StatusCode = StatusCodes.Status404NotFound;
        await WriteEmptyResponse().ConfigureAwait(false);
    }

    public async Task BadRequest()
    {
        StatusCode = StatusCodes.Status400BadRequest;
        await WriteEmptyResponse().ConfigureAwait(false);
    }

    public async Task Unauthorized()
    {
        StatusCode = StatusCodes.Status401Unauthorized;
        await WriteEmptyResponse().ConfigureAwait(false);
    }


    public async Task Forbidden()
    {
        StatusCode = StatusCodes.Status403Forbidden;
        await WriteEmptyResponse().ConfigureAwait(false);
    }

    public async Task InternalServerError(string message)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
        await WriteTextAsync(message, StatusCode.Value).ConfigureAwait(false);
    }

    public async Task Ok()
    {
        StatusCode = StatusCodes.Status200OK;
        await WriteEmptyResponse().ConfigureAwait(false);
    }

    public async Task WriteJsonAsync<T>(T content, int statusCode = 200)
    {
        StatusCode = statusCode;
        StatusCode ??= StatusCodes.Status200OK;

        byte[] jsonResponse = JsonSerializer.SerializeToUtf8Bytes(content);

        SetHttpHeader(HttpHeaderConstants.ContentLength, jsonResponse.Length.ToString());
        SetHttpHeader(HttpHeaderConstants.ContentType, HttpHeaderConstants.ContentTypeApplicationJson);
        await WriteResponseLineAndHeaders().ConfigureAwait(false);
        await WriteBody(jsonResponse).ConfigureAwait(false);
    }

    public async Task WriteTextAsync(string content, int statusCode = 200)
    {
        StatusCode = statusCode;
        StatusCode ??= StatusCodes.Status200OK;

        byte[] resultBytes = Encoding.UTF8.GetBytes(content);

        SetHttpHeader(HttpHeaderConstants.ContentLength, resultBytes.Length.ToString());
        SetHttpHeader(HttpHeaderConstants.ContentType, HttpHeaderConstants.ContentTypeTextPlain);
        await WriteResponseLineAndHeaders().ConfigureAwait(false);
        await WriteBody(resultBytes).ConfigureAwait(false);
    }

    public async Task WriteBytesAsync(byte[] content, int statusCode = 200)
    {
        StatusCode = statusCode;
        StatusCode ??= StatusCodes.Status200OK;

        SetHttpHeader(HttpHeaderConstants.ContentLength, content.Length.ToString());
        await WriteResponseLineAndHeaders().ConfigureAwait(false);
        await WriteBody(content).ConfigureAwait(false);
    }

    private async Task WriteEmptyResponse()
    {
        StatusCode ??= StatusCodes.Status200OK;

        await WriteResponseLineAndHeaders().ConfigureAwait(false);
        await BodyWriter.WriteAsync(ByteConstants.CrLf).ConfigureAwait(false);
    }

    private async Task WriteResponseLineAndHeaders()
    {
        Started = true;

        await BodyWriter.WriteAsync(HttpProtocolVersion.ToBytes(ProtocolVersion)).ConfigureAwait(false);
        await BodyWriter.WriteAsync(ByteConstants.Space).ConfigureAwait(false);
        await BodyWriter.WriteAsync(ReasonPhrases.ToBytes(StatusCode!.Value)).ConfigureAwait(false);
        await BodyWriter.WriteAsync(ByteConstants.CrLf).ConfigureAwait(false);

        if (Headers.Count > 0)
        {
            foreach (var header in Headers)
            {
                await WriteHeader(header.Key, header.Value).ConfigureAwait(false);
            }
        }
    }

    private Task WriteHeader(string header, string value) => WriteHeader(Encoding.ASCII.GetBytes(header), value);

    private async Task WriteHeader(ReadOnlyMemory<byte> header, string value)
    {
        await BodyWriter.WriteAsync(header).ConfigureAwait(false);
        await BodyWriter.WriteAsync(ByteConstants.HeaderDelimiter).ConfigureAwait(false);
        await BodyWriter.WriteAsync(Encoding.ASCII.GetBytes(value)).ConfigureAwait(false);
        await BodyWriter.WriteAsync(ByteConstants.CrLf).ConfigureAwait(false);
    }

    private async Task WriteBody(byte[] content)
    {
        await BodyWriter.WriteAsync(ByteConstants.CrLf).ConfigureAwait(false);
        await BodyWriter.WriteAsync(content);
    }
}