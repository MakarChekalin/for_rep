using System.ComponentModel.DataAnnotations;

namespace Atm.WebApi;

public class AdminOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; } = string.Empty;
}
