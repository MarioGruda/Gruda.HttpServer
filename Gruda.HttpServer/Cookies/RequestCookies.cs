namespace Gruda.HttpServer.Cookies;

// TODO: Implement cookie parsing
public sealed class RequestCookies : Cookies
{
    internal void ParseCookies(string cookies)
    {
        string[] cookiePairs = cookies.Split(';');

        foreach (string cookiePair in cookiePairs)
        {
            string[] cookie = cookiePair.Split('=');

            if (cookie.Length >= 2)
                CookiesStore.Add(cookie[0], cookie[1]);
        }
    }
}