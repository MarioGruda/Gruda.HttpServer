using System.Diagnostics.CodeAnalysis;

namespace Gruda.HttpServer.IdentityManagement;

public class Identity
{
    public List<Claim> Claims { get; init; } = new();

    public bool IsAuthenticated { get; private set; } = true;

    public string? Name
    {
        get { return Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value; }
    }

    public string? Role
    {
        get { return Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value; }
    }

    public bool TryGetClaim(string type, [NotNullWhen(true)] out Claim? claim)
    {
        claim = Claims.FirstOrDefault(c => c.Type == type);
        return claim is not null;
    }

    public Claim? GetClaim(string type) => Claims.FirstOrDefault(c => c.Type == type);
    public void AddClaim(string type, string value) => Claims.Add(new Claim(type, value));

    public void AddClaim(Claim claim) => Claims.Add(claim);

    public void RemoveClaim(string type) => Claims.RemoveAll(c => c.Type == type);

    public bool HasClaim(string type) => Claims.Any(c => c.Type == type);

    public bool HasClaim(string type, string value) => Claims.Any(c => c.Type == type && c.Value == value);
}