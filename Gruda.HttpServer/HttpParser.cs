using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace Gruda.HttpServer;

public static class HttpParser
{
    private const byte Lf = (byte) '\n';
    private const byte Cr = (byte) '\r';
    private const byte Colon = (byte) ':';
    private const byte Space = (byte) ' ';
    private const byte RootUrl = (byte) '/';

    // TODO: Rework!...
    // TODO: add support for query string
    public static async Task<HttpRequest> ParseRequest(PipeReader reader, CancellationToken cancellationToken = default)
    {
        HttpRequest request = new();

        bool requestLineProcessed = false;
        bool headersProcessed = false;

        while (true)
        {
            ReadResult result = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            ReadOnlySequence<byte> resultBuffer = result.Buffer;

            SequencePosition position;

            if (!requestLineProcessed)
            {
                (position, requestLineProcessed) = ProcessHttpRequestLine(ref resultBuffer, request);
                if (!requestLineProcessed)
                {
                    reader.AdvanceTo(position, resultBuffer.End);
                }
                else
                {
                    reader.AdvanceTo(position);
                }

                continue;
            }

            if (!headersProcessed)
            {
                (position, headersProcessed) = ProcessHeaders(ref resultBuffer, request);
                if (!headersProcessed)
                {
                    reader.AdvanceTo(position, resultBuffer.End);
                }
                else
                {
                    reader.AdvanceTo(position);
                    break;
                }
            }
        }

        // Body should not be buffered
        if (request.Method is "POST" or "PUT" or "PATCH")
        {
            // transfer-encoding: chunked not supported
            if (request.Headers.ContentLength == 0)
                throw new Exception("Content-Length header is required.");

            while (true)
            {
                ReadResult result = await reader.ReadAsync(cancellationToken);
                ReadOnlySequence<byte> resultBuffer = result.Buffer;

                if (resultBuffer.Length == request.Headers.ContentLength)
                {
                    request.BufferedBody = resultBuffer.ToArray();
                    reader.AdvanceTo(resultBuffer.End);
                    break;
                }

                reader.AdvanceTo(resultBuffer.Start, resultBuffer.End);
            }
        }

        return request;

        static (SequencePosition, bool RequestLineProcess) ProcessHttpRequestLine(
            ref ReadOnlySequence<byte> resultBuffer, HttpRequest httpRequest)
        {
            if (resultBuffer.PositionOf(Lf) is null)
                return (resultBuffer.Start, false);

            SequenceReader<byte> sequenceReader = new(resultBuffer);

            sequenceReader.TryReadTo(out ReadOnlySequence<byte> line, Space);
            httpRequest.Method = HttpMethods.Parse(line.FirstSpan);

            sequenceReader.TryReadTo(out line, Space);
            httpRequest.Url = Encoding.ASCII.GetString(line);

            sequenceReader.TryReadTo(out line, Lf);
            httpRequest.ProtocolVersion = HttpProtocolVersion.Parse(line.Slice(0, line.Length - 1).FirstSpan);

            return (sequenceReader.Position, true);
        }

        static (SequencePosition, bool HeadersProcessed) ProcessHeaders(ref ReadOnlySequence<byte> resultBuffer,
            HttpRequest request)
        {
            SequenceReader<byte> sequenceReader = new(resultBuffer);

            while (sequenceReader.TryReadTo(out ReadOnlySequence<byte> line, Lf))
            {
                // Body starts after the empty line
                if (line.Length == 1)
                    return (sequenceReader.Position, true);

                SequenceReader<byte> headerReader = new(line);
                if (!headerReader.TryReadTo(out ReadOnlySequence<byte> headerName, Colon))
                    return (sequenceReader.Position, false);

                if (!headerReader.TryReadTo(out ReadOnlySequence<byte> headerValue, Cr))
                    return (sequenceReader.Position, false);

                request.AddHttpHeader(Encoding.ASCII.GetString(headerName),
                    Encoding.ASCII.GetString(headerValue.Slice(1)));
            }

            return (sequenceReader.Position, false);
        }
    }
}