namespace Gruda.HttpServer;

public class Endpoint
{
    internal Func<HttpContext, Task> RouteDelegate { get; init; }

    internal Dictionary<Type, object>? Metadata;

    public Endpoint(Func<HttpContext, Task> routeDelegate)
    {
        RouteDelegate = routeDelegate;
    }

    public void SetMetadata<T>(T metadata) where T : class
    {
        Metadata ??= new Dictionary<Type, object>();
        Metadata[typeof(T)] = metadata;
    }

    public void RemoveMetadata<T>() where T : class
    {
        Metadata?.Remove(typeof(T));
    }
}