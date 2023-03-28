var builder = WebApplication.CreateBuilder();

var app = builder.Build();

app.MapGet("/hello", () => """Console app says "Hello, World!" """);

app.Run();