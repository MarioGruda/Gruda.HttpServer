namespace Gruda.HttpServer.Transport.KestrelBased;

// From: https://github.com/dotnet/aspnetcore/blob/main/src/Servers/Connections.Abstractions/src/Exceptions/ConnectionResetException.cs
/// <summary>
/// An exception thrown when the connection is reset.
/// </summary>
public class ConnectionResetException : IOException
{
    /// <summary>
    /// Initializes a new instance of <see cref="ConnectionResetException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ConnectionResetException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConnectionResetException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="inner">The underlying <see cref="Exception"/>.</param>
    public ConnectionResetException(string message, Exception inner) : base(message, inner)
    {
    }
}