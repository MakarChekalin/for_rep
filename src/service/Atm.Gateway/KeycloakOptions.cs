using System.ComponentModel.DataAnnotations;

namespace Atm.Gateway;

public class KeycloakOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Authority { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string Audience { get; set; } = string.Empty;

    public bool RequireHttpsMetadata { get; set; } = true;
}
