using System.Text;

namespace Gruda.HttpServer;

public static class HttpProtocolVersion
{
    private static readonly byte[] Http1_0 = "HTTP/1.0"u8.ToArray();
    private static readonly byte[] Http1_1 = "HTTP/1.1"u8.ToArray();
    private static readonly byte[] Http2_0 = "HTTP/2.0"u8.ToArray();
    private static readonly byte[] Http3_0 = "HTTP/3.0"u8.ToArray();

    public static string Parse(ReadOnlySpan<byte> version) =>
        version switch
        {
            _ when version.SequenceEqual(Http1_0) => "HTTP/1.0",
            _ when version.SequenceEqual(Http1_1) => "HTTP/1.1",
            _ when version.SequenceEqual(Http2_0) => "HTTP/2.0",
            _ when version.SequenceEqual(Http3_0) => "HTTP/3.0",
            _ => "UNKNOWN"
        };

    public static byte[] ToBytes(string version) =>
        version switch
        {
            "HTTP/1.0" => Http1_0,
            "HTTP/1.1" => Http1_1,
            "HTTP/2.0" => Http2_0,
            "HTTP/3.0" => Http3_0,
            _ => Encoding.ASCII.GetBytes(version)
        };
}