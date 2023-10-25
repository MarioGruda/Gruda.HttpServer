namespace Gruda.HttpServer.IdentityManagement;

public static class ClaimTypes
{
    public const string Name = "Name";
    public const string Role = "Role";
}

public record Claim(string Type, string Value);