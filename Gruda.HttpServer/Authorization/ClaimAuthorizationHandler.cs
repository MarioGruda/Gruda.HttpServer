using Gruda.HttpServer.IdentityManagement;

namespace Gruda.HttpServer.Authorization;

public class ClaimAuthorizationHandler : IAuthorizationHandler
{
    private List<ClaimToAuthorize>? _requiredClaims;

    public void AddRequiredClaim(string key, string value)
    {
        _requiredClaims ??= new();
        _requiredClaims.Add(new ClaimToAuthorize(key) {ConstantValue = value});
    }

    public void AddRequiredClaim(string key, Func<string, bool> predicate)
    {
        _requiredClaims ??= new();
        _requiredClaims.Add(new ClaimToAuthorize(key) {Predicate = predicate});
    }

    public Task<AuthorizationResult> HandleAsync(HttpContext context)
    {
        if (_requiredClaims is null)
            return Task.FromResult(AuthorizationResult.Success);

        var user = context.Request.Identity;

        foreach (var requiredClaim in _requiredClaims)
        {
            string key = requiredClaim.Key;
            if (!user!.TryGetClaim(key, out Claim? claim))
            {
                return Task.FromResult(AuthorizationResult.Failure);
            }

            if (requiredClaim.Predicate is not null)
            {
                if (!requiredClaim.Predicate(claim.Value))
                    return Task.FromResult(AuthorizationResult.Failure);
            }
            else
            {
                if (requiredClaim.ConstantValue != claim.Value)
                    return Task.FromResult(AuthorizationResult.Failure);
            }
        }

        return Task.FromResult(AuthorizationResult.Success);
    }

    private class ClaimToAuthorize
    {
        public string Key { get; init; }
        public string? ConstantValue { get; init; }
        public Func<string, bool>? Predicate { get; init; }

        public ClaimToAuthorize(string key)
        {
            Key = key;
        }
    }
}