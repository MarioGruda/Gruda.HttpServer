using System.Security.Cryptography;
using System.Text;

namespace Gruda.HttpServer.Security;

// TODO: Remove static and make it configurable
public static class DataProtector
{
    private static readonly byte[] Key = RandomNumberGenerator.GetBytes(16);
    
    public static byte[] HmacSha256(string data)
    {
        using HMACSHA256 hmac = new(Key);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
    }
}