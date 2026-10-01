using System.ComponentModel.DataAnnotations;

namespace Atm.Gateway.DTO;

public record LoginAdminRequest(
    [Required] string Password);
