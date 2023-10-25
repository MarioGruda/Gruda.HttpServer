using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace Gruda.HttpServer.Transport.KestrelBased;

// From: https://github.com/dotnet/aspnetcore/blob/main/src/Servers/Kestrel/Transport.Sockets/src/Internal/SocketOperationResult.cs
internal readonly struct SocketOperationResult
{
    public readonly SocketException? SocketError;

    public readonly int BytesTransferred;

    [MemberNotNullWhen(true, nameof(SocketError))]
    public readonly bool HasError => SocketError != null;

    public SocketOperationResult(int bytesTransferred)
    {
        SocketError = null;
        BytesTransferred = bytesTransferred;
    }

    public SocketOperationResult(SocketException exception)
    {
        SocketError = exception;
        BytesTransferred = 0;
    }
}