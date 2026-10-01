using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace Atm.Gateway;

public class KeycloakRolesClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || identity.HasClaim(claim => claim.Type == ClaimTypes.Role))
            return Task.FromResult(principal);

        string? realmAccess = principal.FindFirst("realm_access")?.Value;

        if (realmAccess == null)
            return Task.FromResult(principal);

        foreach (string role in ParseRoles(realmAccess))
            identity.AddClaim(new Claim(ClaimTypes.Role, role));

        return Task.FromResult(principal);
    }

    private static IEnumerable<string> ParseRoles(string realmAccessJson)
    {
        using var document = JsonDocument.Parse(realmAccessJson);

        if (!document.RootElement.TryGetProperty("roles", out JsonElement roles))
            yield break;

        foreach (JsonElement role in roles.EnumerateArray())
        {
            string? value = role.GetString();

            if (value != null)
                yield return value;
        }
    }
}
