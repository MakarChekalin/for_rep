using System.ComponentModel.DataAnnotations;

namespace Atm.Gateway;

public class GrpcOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Address { get; set; } = string.Empty;
}
