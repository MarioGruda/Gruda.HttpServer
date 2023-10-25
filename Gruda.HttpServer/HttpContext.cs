using Gruda.HttpServer.IdentityManagement;

namespace Gruda.HttpServer;

public sealed class HttpContext
{
    public HttpRequest Request { get; init; }
    public HttpResponse Response { get; init; }

    internal Endpoint Endpoint { get; set; } = null!;
    internal Dictionary<Type, object>? Metadata => Endpoint.Metadata;

    public HttpContext(HttpRequest request, HttpResponse response)
    {
        Request = request;
        Response = response;
    }

    public bool TryGetMetadata<T>(out T metadata) where T : class
    {
        metadata = default!;

        if (Metadata is null)
            return false;

        if (!Metadata.TryGetValue(typeof(T), out object? metadataObject))
        {
            return false;
        }

        metadata = (T) metadataObject;
        return true;
    }

    public void SignIn(Identity identity)
    {
        IdentityManager.SignIn(identity, this);
    }

    public void SignOut()
    {
        IdentityManager.SignOut(this);
    }
}