
using WebEmpresa.Components;

var builder = WebApplication.CreateBuilder(args);

// Razor Components + Interactive Server
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// HttpClient para consumir el API
builder.Services.AddHttpClient("ApiEmpresa", client =>
{
    client.BaseAddress = new Uri("http://localhost:5164/");
});

// Permitir inyectar HttpClient
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ApiEmpresa");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

// Encabezados de seguridad
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        var headers = context.Response.Headers;

        headers["X-Content-Type-Options"] = "nosniff";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["X-Frame-Options"] = "DENY";

        // CSP inicial compatible con Blazor Interactive Server.
        // Se deberá endurecer después de comprobar
        // los recursos que necesita la aplicación.
        headers["Content-Security-Policy"] =
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data:; " +
            "font-src 'self' data:; " +
            "connect-src 'self' ws: wss:; " +
            "form-action 'self'; " +
            "object-src 'none'; " +
            "base-uri 'self'; " +
            "frame-ancestors 'none'";

        return Task.CompletedTask;
    });

    await next();
});

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
