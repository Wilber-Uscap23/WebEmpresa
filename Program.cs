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

// Para poder usar @inject HttpClient Http
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

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();