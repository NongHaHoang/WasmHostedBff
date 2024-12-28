using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource("dob", ["dob"]),
            new IdentityResource("phone_number", ["phone_number"]),
            new IdentityResource("avatar_url", ["avatar_url"]),
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            // local API
            new ApiScope(IdentityServerConstants.LocalApi.ScopeName),
        ];

    public static IEnumerable<Client> Clients =>
        [
            // interactive hosted local client
            new Client
            {
                ClientId = "wasm.hosted",
                ClientName = "Wasm Hosted",
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                ClientUri = "https://localhost:7160",

                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,

                RedirectUris = { "https://localhost:7160/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:7160/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:7160/signout-callback-oidc" },

                AllowedCorsOrigins = { "https://localhost:7160" },

                AllowOfflineAccess = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,

                AllowedScopes = { "openid", "profile", "email", "dob", "phone_number", "avatar_url", IdentityServerConstants.LocalApi.ScopeName }
            }
        ];
}