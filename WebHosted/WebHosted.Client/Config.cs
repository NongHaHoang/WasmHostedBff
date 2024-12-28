using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Net;
using System.Security.Claims;

namespace WebHosted.Client;

public static class Config
{
}

public class BffAuthenticationStateProvider(HttpClient client, ILogger<BffAuthenticationStateProvider> logger) : AuthenticationStateProvider
{
    private static readonly TimeSpan UserCacheRefreshInterval = TimeSpan.FromSeconds(60);
    private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
    private ClaimsPrincipal _cachedUser = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return new AuthenticationState(await GetUser());
    }

    private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = true)
    {
        var now = DateTimeOffset.Now;
        if (useCache && now < _userLastCheck + UserCacheRefreshInterval)
        {
            logger.LogDebug("Taking user from cache");
            return _cachedUser;
        }

        logger.LogDebug("Fetching user");
        _cachedUser = await FetchUser();
        _userLastCheck = now;

        return _cachedUser;
    }

    record ClaimRecord(string Type, object Value);

    private async Task<ClaimsPrincipal> FetchUser()
    {
        try
        {
            logger.LogInformation("Fetching user information.");
            var response = await client.GetAsync("bff/user?slide=false");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var claims = await response.Content.ReadFromJsonAsync<List<ClaimRecord>>();

                var identity = new ClaimsIdentity(nameof(BffAuthenticationStateProvider), "name", "role");

                foreach (var claim in claims!)
                {
                    identity.AddClaim(new Claim(claim.Type, claim.Value.ToString()!));
                }

                return new ClaimsPrincipal(identity);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Fetching user failed.");
        }

        return new ClaimsPrincipal(new ClaimsIdentity());
    }
}

public class AntiforgeryHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("X-CSRF", "1");
        return base.SendAsync(request, cancellationToken);
    }
}