using System.Net.Sockets;

namespace Gruda.HttpServer.Transport.Pipelines.Sockets.Unofficial;

public class PipelineSocketConnectionFactory : ISocketConnectionFactory
{
    public ISocketConnection CreateConnection(Socket socket)
    {
       // https://github.com/mgravell/Pipelines.Sockets.Unofficial/tree/main
       throw new NotImplementedException();
    }
}