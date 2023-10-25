namespace Gruda.HttpServer.Authorization;

public class AuthorizationOptions
{
    public List<IAuthorizationHandler> Handlers { get; } = new();

    public void RequireRole(string role)
    {
        var claimHandler = GetClaimAuthorizationHandler();
        claimHandler.AddRequiredClaim("role", role);
    }

    public void RequireClaim(string claim, string value)
    {
        var claimHandler = GetClaimAuthorizationHandler();
        claimHandler.AddRequiredClaim(claim, value);
    }

    public void RequireClaim(string claim, Func<string, bool> predicate)
    {
        var claimHandler = GetClaimAuthorizationHandler();
        claimHandler.AddRequiredClaim(claim, predicate);
    }

    private ClaimAuthorizationHandler GetClaimAuthorizationHandler()
    {
        var claimHandler =
            Handlers.FirstOrDefault(h => h.GetType() == typeof(ClaimAuthorizationHandler)) as ClaimAuthorizationHandler;
        if (claimHandler is null)
        {
            claimHandler = new ClaimAuthorizationHandler();
            Handlers.Add(claimHandler);
        }

        return claimHandler;
    }
}