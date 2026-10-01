using System.ComponentModel.DataAnnotations;

namespace Atm.Gateway.DTO;

public record AssignAccountantRequest(
    [Required] Guid SessionKey,
    [Required] string AccountantUserId);
