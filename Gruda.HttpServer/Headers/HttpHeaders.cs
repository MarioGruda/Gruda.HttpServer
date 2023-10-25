using System.Collections;
using System.Text;

namespace Gruda.HttpServer.Headers;

public abstract class HttpHeaders : IEnumerable<KeyValuePair<string, string>>
{
    private protected readonly Dictionary<string, string> Headers = new();

    public int Count => Headers.Count;

    public string? this[string key]
    {
        get
        {
            if (Headers.TryGetValue(key, out string? value))
                return value;

            return null;
        }
        set => Headers[key] = value ?? string.Empty;
    }

    internal void Set(string key, string value) => this[key] = value;

    internal void Remove(string key) => Headers.Remove(key);

    internal bool Contains(string key) => Headers.ContainsKey(key);

    public override string ToString()
    {
        StringBuilder builder = new();

        foreach (KeyValuePair<string, string> header in Headers)
        {
            builder.Append($"{header.Key}: {header.Value}\r\n");
        }

        return builder.ToString();
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => Headers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}