using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Atm.Gateway;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services)
    {
        services.AddOptions<KeycloakOptions>()
            .BindConfiguration("Authentication:Keycloak")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<KeycloakOptions>>((options, keycloakOptions) =>
            {
                options.Authority = keycloakOptions.Value.Authority;
                options.RequireHttpsMetadata = keycloakOptions.Value.RequireHttpsMetadata;
                options.MapInboundClaims = false;
                options.TokenValidationParameters.ValidAudience = keycloakOptions.Value.Audience;
            });

        services.AddAuthorization();
        services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>();

        return services;
    }
}
