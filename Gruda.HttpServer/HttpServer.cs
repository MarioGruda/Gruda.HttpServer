using System.Net;
using System.Net.Sockets;
using Gruda.HttpServer.Transport;
using Gruda.HttpServer.Transport.KestrelBased;

namespace Gruda.HttpServer;

internal class HttpServer
{
    private readonly ISocketConnectionFactory _socketConnectionFactory;
    private readonly IPAddress _address;
    private readonly int _port;
    private readonly Socket _socket;
    private CancellationTokenSource _cancellationTokenSource = null!;
    private Task? _task = null!;

    internal HttpServer(IPAddress address, int port)
    {
        _address = address;
        _port = port;
        _socketConnectionFactory = new KestrelSocketConnectionFactory();
        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
    }

    internal Task Stop()
    {
        _cancellationTokenSource.Cancel();
        return _task ?? Task.CompletedTask;
    }

    internal Task StartAsync(Func<HttpContext, Task> execRequest, CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        if (cancellationToken != default)
        {
            cancellationToken.Register(() => _cancellationTokenSource.Cancel());
        }

        var socket = _socket;
        socket.Bind(new IPEndPoint(_address, _port));
        socket.Listen();

        _task = Task.Run(async () =>
        {
            while (true)
            {
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                Socket clientSocket = await socket.AcceptAsync(_cancellationTokenSource.Token)
                    .ConfigureAwait(false);

                _ = Task.Run(() =>
                {
                    // Only apply no delay to Tcp based endpoints
                    if (clientSocket.LocalEndPoint is IPEndPoint)
                    {
                        clientSocket.NoDelay = true;
                    }

                    return ProcessHttpConnection(_socketConnectionFactory.CreateConnection(clientSocket), execRequest);
                });
            }
        }, cancellationToken);

        return _task;
    }

    private async Task ProcessHttpConnection(ISocketConnection socketConnection, Func<HttpContext, Task> execRequest)
    {
        await using (socketConnection)
        {
            HttpRequest request = await HttpParser.ParseRequest(socketConnection.Transport.Input).ConfigureAwait(false);
            HttpContext context = new(request,
                new HttpResponse(socketConnection.Transport.Output, request.ProtocolVersion));

            await execRequest(context).ConfigureAwait(false);
        }
    }
}