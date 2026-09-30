using System.ComponentModel.DataAnnotations;

namespace Atm.WebApi.DTO;

public record AmountRequest(
    [Required] Guid SessionKey,
    [Range(0.01, double.MaxValue)] decimal Amount); // чтобы не снимали <0