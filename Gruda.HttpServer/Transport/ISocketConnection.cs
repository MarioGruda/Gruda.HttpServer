using System.IO.Pipelines;

namespace Gruda.HttpServer.Transport;

public interface ISocketConnection : IAsyncDisposable
{
    public IDuplexPipe Transport { get; }
    
    void Start();
}