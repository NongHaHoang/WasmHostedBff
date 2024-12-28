using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityServer;

public class CustomProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory) : ProfileService<ApplicationUser>(userManager, claimsFactory)
{
    protected override async Task GetProfileDataAsync(ProfileDataRequestContext context, ApplicationUser user)
    {
        var principal = await GetUserClaimsAsync(user);
        var identity = (ClaimsIdentity)principal.Identity!;
        if (user.DOB is not null)
            identity.AddClaim(new Claim("dob", user.DOB.Value.ToString()));
        if (user.PhoneNumber is not null)
            identity.AddClaim(new Claim("phone_number", user.PhoneNumber));
        if (user.AvatarUrl is not null)
            identity.AddClaim(new Claim("avatar_url", user.AvatarUrl));
        context.AddRequestedClaims(principal.Claims);
    }
}