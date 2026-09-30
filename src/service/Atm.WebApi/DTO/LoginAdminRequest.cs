using System.ComponentModel.DataAnnotations;

namespace Atm.WebApi.DTO;

public record LoginAdminRequest(
    [Required] string Password);
