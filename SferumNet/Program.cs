using System.Net;
using SferumNet.Components;
using MudBlazor.Services;
using SferumNet.Database;
using SferumNet.Services;
using SferumNet.Services.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddTransient<SferumNetContext>();
builder.Services.AddScoped<DbLogger>();
builder.Services.AddSingleton<IScenarioConfigurator, ScenarioConfigurator>();

builder.WebHost.ConfigureKestrel((httpClient, options) =>
{
    options.Listen(IPAddress.Any, 5001);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();