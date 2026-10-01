using System.ComponentModel.DataAnnotations;

namespace Atm.Gateway.DTO;

public record AmountRequest(
    [Required] Guid SessionKey,
    [Range(0.01, double.MaxValue)] decimal Amount);
