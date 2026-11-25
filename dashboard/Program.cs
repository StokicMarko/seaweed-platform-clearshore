using Dashboard.Components;
using Dashboard.Services;
using Dashboard.Services.ChangeRequests;
using Dashboard.Services.Media;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Dashboard helper for media requests.
builder.Services.AddHttpClient<HttpMediaService>(
    client =>
    {
        client.Timeout = TimeSpan.FromMinutes(15);

        client.BaseAddress = new Uri(
            builder.Configuration["BackendUrl"]!);
    });

builder.Services.AddTransient<IMediaReader>(services =>
    services.GetRequiredService<HttpMediaService>());

builder.Services.AddTransient<IUploadRulesService>(services =>
    services.GetRequiredService<HttpMediaService>());

builder.Services.AddTransient<IMediaWriter>(services =>
    services.GetRequiredService<HttpMediaService>());

// Dashboard helper for technology choices.
builder.Services.AddHttpClient<
    ITechnologyService,
    HttpTechnologyService>(
    client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration["BackendUrl"]!);
    });

// Dashboard helper for Main Admin review requests.
builder.Services.AddHttpClient<
    IChangeRequestReviewService,
    HttpChangeRequestReviewService>(
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