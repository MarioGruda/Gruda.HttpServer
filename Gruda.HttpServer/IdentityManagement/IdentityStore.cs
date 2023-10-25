using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Gruda.HttpServer.IdentityManagement;

// TODO: Remove static
public static class IdentityStore
{
    private static ConcurrentDictionary<string, Identity>? _identities;

    public static void AddIdentity(string id, Identity identity)
    {
        _identities ??= new ConcurrentDictionary<string, Identity>();
        _identities.TryAdd(id, identity);
    }

    public static void RemoveIdentity(string id)
    {
        if (_identities is null)
            return;

        _identities.TryRemove(id, out _);
    }

    public static Identity? GetIdentity(string id) => _identities?[id];

    public static bool TryGetIdentity(string id, [NotNullWhen(true)] out Identity? identity)
    {
        identity = null;
        return _identities?.TryGetValue(id, out identity) ?? false;
    }
}