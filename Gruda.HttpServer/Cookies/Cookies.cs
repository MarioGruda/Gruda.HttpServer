using System.Collections;

namespace Gruda.HttpServer.Cookies;

public abstract class Cookies : IEnumerable<KeyValuePair<string, string>>
{
    protected readonly Dictionary<string, string> CookiesStore = new();

    public int Count => CookiesStore.Count;

    public string? this[string key]
    {
        get
        {
            if (CookiesStore.TryGetValue(key, out var value))
                return value;

            return null;
        }
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return CookiesStore.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}