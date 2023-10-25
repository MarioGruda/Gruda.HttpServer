namespace Gruda.HttpServer.IdentityManagement;

public static class HostBuilderAuthExtensions
{
    public static HttpHostBuilder UseCookieAuthentication(this HttpHostBuilder builder)
    {
        builder.Use<CookieAuthenticationMiddleware>();
        return builder;
    }
}