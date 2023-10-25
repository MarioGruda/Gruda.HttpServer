using System.Security.Cryptography;
using Gruda.HttpServer.Security;

namespace Gruda.HttpServer.IdentityManagement;

// TODO: Remove static
public static class IdentityManager
{
    public static void SignIn(Identity identity, HttpContext context)
    {
        string id = RandomNumberGenerator.GetHexString(32);
        IdentityStore.AddIdentity(id, identity);

        byte[] hash = DataProtector.HmacSha256(id);
        context.Response.AppendCookie("auth", $"{id}.{WebEncoders.Base64UrlEncode(hash)}");

        context.Request.Identity = identity;
    }
    
    public static void SignOut(HttpContext context)
    {
        string id = GetIdentityStoreId(context);
        if (string.IsNullOrWhiteSpace(id))
            return;

        IdentityStore.RemoveIdentity(id);
        context.Request.Identity = null;
        context.Response.RemoveCookie("auth");
    }

    public static Identity? GetIdentity(HttpContext context)
    {
        string id = GetIdentityStoreId(context);
        return IdentityStore.TryGetIdentity(id, out Identity? identity) ? identity : null;
    }

    private static string GetIdentityStoreId(HttpContext context)
    {
        string? authCookie = context.Request.Cookies["auth"];
        if (authCookie is not null)
        {
            string[] parts = authCookie.Split('.');
            if (parts.Length != 2)
                return string.Empty;

            string id = parts[0];
            byte[] hash = DataProtector.HmacSha256(id);
            if (hash.SequenceEqual(WebEncoders.Base64UrlDecode(parts[1])))
            {
                return id;
            }
        }

        return string.Empty;
    }
}