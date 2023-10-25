using System.Net;
using Gruda.HttpServer;
using Gruda.HttpServer.Authorization;
using Gruda.HttpServer.IdentityManagement;

Console.WriteLine("Hello, World!");

HttpHost host = new HttpHostBuilder(IPAddress.Parse("127.0.0.1"), 5001)
    .UseCookieAuthentication()
    .UseAuthorization()
    // .Use<TestMiddleware>()
    .Build();

host.MapGet("/", (context) => context.Response.WriteTextAsync("Hello, World!"));

host.MapGet("/error",
    (context) => throw new Exception("This is an exception thrown from the handler."));

host.MapGet("/signin", (context) =>
{
    context.SignIn(new Identity()
    {
        Claims = new()
        {
            new Claim("Name", "Gruda"),
            new Claim("ID", "123456"),
        }
    });
    return Task.CompletedTask;
});

host.MapGet("/signout", (context) =>
{
    context.SignOut();
    return Task.CompletedTask;
});

host.MapGet("/ping", async (context) => await context.Response.WriteTextAsync("Pong"));

host.MapGet("/user", async (context) => await context.Response.WriteJsonAsync(context.Request.Identity))
    .RequireAuthorization();

host.MapPost("/todo", async context =>
{
    Todo? todo = context.Request.ReadBodyAsJson<Todo>();
    if (todo is null)
    {
        await context.Response.BadRequest();
        return;
    }

    await context.Response.WriteJsonAsync(todo);
}).RequireAuthorization(options =>
{
    options.RequireRole("Admin");
    options.RequireClaim("age", age => int.Parse(age) > 18);
});

host.MapGetFallback(async (context) =>
    await context.Response.WriteTextAsync("Fallback Handler 404", StatusCodes.Status418ImATeapot));

await host.StartAsync();

record Todo(string Name, int Priority, bool Done);