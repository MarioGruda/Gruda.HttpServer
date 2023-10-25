namespace Gruda.HttpServer.Authorization;

public interface IAuthorizationHandler
{
    Task<AuthorizationResult> HandleAsync(HttpContext context);
}

public enum AuthorizationResult
{
    Success,
    Failure
}

internal class AuthorizationHandler
{
    private readonly List<IAuthorizationHandler>? _handlers;

    public AuthorizationHandler(List<IAuthorizationHandler>? handlers)
    {
        _handlers = handlers;
    }

    public async Task<AuthorizationResult> HandleAsync(HttpContext context)
    {
        if (context.Request.Identity is null)
        {
            return AuthorizationResult.Failure;
        }

        if (_handlers is {Count: > 0})
        {
            foreach (var handler in _handlers)
            {
                var result = await handler.HandleAsync(context);
                if (result == AuthorizationResult.Failure)
                {
                    return AuthorizationResult.Failure;
                }
            }
        }

        return AuthorizationResult.Success;
    }
}