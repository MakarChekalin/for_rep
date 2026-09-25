using System.ComponentModel.DataAnnotations;

namespace Atm.Infrastructure;

public class DatabaseOptions
{
    [Required(AllowEmptyStrings = false)]
    public string ConnectionString { get; set; } = string.Empty;
}
