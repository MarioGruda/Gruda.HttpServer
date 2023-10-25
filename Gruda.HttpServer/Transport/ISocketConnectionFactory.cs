using System.Net.Sockets;

namespace Gruda.HttpServer.Transport;

public interface ISocketConnectionFactory
{
    public ISocketConnection CreateConnection(Socket socket);
}