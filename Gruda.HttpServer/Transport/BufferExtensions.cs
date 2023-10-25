using System.Runtime.InteropServices;

namespace Gruda.HttpServer.Transport;

// From: https://github.com/dotnet/aspnetcore/blob/main/src/Servers/Kestrel/Transport.Sockets/src/Internal/BufferExtensions.cs
internal static class BufferExtensions
{
    public static ArraySegment<byte> GetArray(this Memory<byte> memory)
    {
        return ((ReadOnlyMemory<byte>) memory).GetArray();
    }

    public static ArraySegment<byte> GetArray(this ReadOnlyMemory<byte> memory)
    {
        if (!MemoryMarshal.TryGetArray(memory, out var result))
        {
            throw new InvalidOperationException("Buffer backed by array was expected");
        }

        return result;
    }
}