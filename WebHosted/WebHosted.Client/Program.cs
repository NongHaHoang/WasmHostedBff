using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebHosted.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();

// HTTP client configuration
builder.Services.AddTransient<AntiforgeryHandler>();
builder.Services.AddHttpClient("backend", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AntiforgeryHandler>();
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("backend"));

var apiAddress = "https://localhost:7075";
var identityServerAddress = "https://localhost:5001";

builder.Services.AddHttpClient("ISAPI", client => client.BaseAddress = new Uri(identityServerAddress))
    .AddHttpMessageHandler<AntiforgeryHandler>();
builder.Services.AddHttpClient("WebAPI", client => client.BaseAddress = new Uri(apiAddress))
    .AddHttpMessageHandler<AntiforgeryHandler>();

builder.Services
        .AddKeyedScoped("ISAPI", (sp, _) => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ISAPI"))
        .AddKeyedScoped("WebAPI", (sp, _) => sp.GetRequiredService<IHttpClientFactory>().CreateClient("WebAPI"));

await builder.Build().RunAsync();
