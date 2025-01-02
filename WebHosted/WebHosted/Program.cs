using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using WebHosted.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();

builder.Services.AddBff(options =>
    {
        options.RevokeRefreshTokenOnLogout = false;
    })
    .AddRemoteApis();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie("cookie", options =>
    {
        options.Cookie.Name = "__Host-blazor";
        options.Cookie.SameSite = SameSiteMode.Strict;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "https://localhost:5001";
        options.ClientId = "wasm.hosted";
        options.ClientSecret = "secret";
        options.ResponseType = "code";
        options.ResponseMode = "query";

        options.CallbackPath = "/signin-oidc";
        options.SignedOutCallbackPath = "/signout-oidc";

        options.Scope.Clear();
        options.Scope.Add("offline_access");
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.Scope.Add("dob");
        options.Scope.Add("phone_number");
        options.Scope.Add("avatar_url");

        options.Scope.Add("IdentityServerApi");

        options.ClaimActions.MapJsonKey("dob", "dob");
        options.ClaimActions.MapJsonKey("phone_number", "phone_number");
        options.ClaimActions.MapJsonKey("avatar_url", "avatar_url");
        options.ClaimActions.MapJsonKey("IdentityServerApi", "IdentityServerApi");

        options.MapInboundClaims = false;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseAuthentication();
app.UseBff();
app.UseAuthorization();

app.MapBffManagementEndpoints();

app.MapControllers()
    .RequireAuthorization()
    .AsBffApiEndpoint();

app.MapRemoteBffApiEndpoint("/api/WeatherForecast", "https://localhost:7075")
    .RequireAccessToken(Duende.Bff.TokenType.User)
    .SkipAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WebHosted.Client._Imports).Assembly);

app.Run();