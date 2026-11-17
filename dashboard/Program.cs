using Dashboard.Components;
using Dashboard.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Dashboard helper for media requests.
builder.Services.AddHttpClient<IMediaService, HttpMediaService>(
    client =>
    {
        client.Timeout = TimeSpan.FromMinutes(15);

        client.BaseAddress = new Uri(
            builder.Configuration["BackendUrl"]!);
    });

// Dashboard helper for technology choices.
builder.Services.AddHttpClient<HttpTechnologyService>(
    client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration["BackendUrl"]!);
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();