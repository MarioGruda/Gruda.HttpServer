namespace Gruda.HttpServer;

internal static class HttpMethods
{
    private static readonly byte[] Get = "GET"u8.ToArray();
    private static readonly byte[] Post = "POST"u8.ToArray();
    private static readonly byte[] Put = "PUT"u8.ToArray();
    private static readonly byte[] Patch = "PATCH"u8.ToArray();
    private static readonly byte[] Delete = "DELETE"u8.ToArray();
    private static readonly byte[] Head = "HEAD"u8.ToArray();
    private static readonly byte[] Options = "OPTIONS"u8.ToArray();

    public static string Parse(ReadOnlySpan<byte> method) =>
        method switch
        {
            _ when method.SequenceEqual(Get) => "GET",
            _ when method.SequenceEqual(Post) => "POST",
            _ when method.SequenceEqual(Put) => "PUT",
            _ when method.SequenceEqual(Patch) => "PATCH",
            _ when method.SequenceEqual(Delete) => "DELETE",
            _ when method.SequenceEqual(Head) => "HEAD",
            _ when method.SequenceEqual(Options) => "OPTIONS",
            _ => "UNKNOWN"
        };
}