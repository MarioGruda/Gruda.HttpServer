namespace Gruda.HttpServer.Authorization;

public static class HostBuilderAuthzExtensions
{
    public static HttpHostBuilder UseAuthorization(this HttpHostBuilder builder)
    {
        builder.Use<AuthorizationMiddleware>();
        return builder;
    }


    public static Endpoint RequireAuthorization(this Endpoint endpoint, Action<AuthorizationOptions>? configure = null)
    {
        var options = new AuthorizationOptions();
        configure?.Invoke(options);
        endpoint.SetMetadata(new AuthorizationHandler(options.Handlers));
        return endpoint;
    }
}