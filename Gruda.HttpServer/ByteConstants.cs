namespace Gruda.HttpServer;

internal static class ByteConstants
{
    internal static readonly ReadOnlyMemory<byte> Space = " "u8.ToArray();
    internal static readonly ReadOnlyMemory<byte> HeaderDelimiter = ": "u8.ToArray();
    internal static readonly ReadOnlyMemory<byte> CrLf = "\r\n"u8.ToArray();
}