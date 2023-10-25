using System.IO.Pipelines;
using System.Net.Sockets;
using Gruda.HttpServer.Transport.KestrelBased.MemoryPool;

namespace Gruda.HttpServer.Transport.KestrelBased;

public class KestrelSocketConnectionFactory : ISocketConnectionFactory
{
    const int MaxReadBufferSize = 1024 * 1024;
    const int MaxWriteBufferSize = 64 * 1024;

    private static readonly IOQueue TransportScheduler = new();
    private static readonly PipeScheduler ApplicationScheduler = PipeScheduler.ThreadPool;
    private static readonly PinnedBlockMemoryPool MemoryPool = new PinnedBlockMemoryPool();

    private static readonly PipeOptions InputOptions = new(MemoryPool, ApplicationScheduler,
        TransportScheduler,
        MaxReadBufferSize, MaxReadBufferSize / 2, useSynchronizationContext: false);

    private static readonly PipeOptions OutputOptions = new(MemoryPool, TransportScheduler,
        ApplicationScheduler,
        MaxWriteBufferSize, MaxWriteBufferSize / 2, useSynchronizationContext: false);

    private static readonly SocketSenderPool SenderPool = new(PipeScheduler.Inline);


    public ISocketConnection CreateConnection(Socket socket)
    {
        var socketConnection = new KestrelSocketConnection(socket, MemoryPool, SenderPool.Scheduler, SenderPool,
            InputOptions, OutputOptions);

        socketConnection.Start();

        return socketConnection;
    }
}