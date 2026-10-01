using System.ComponentModel.DataAnnotations;

namespace Atm.Gateway.DTO;

public record CreateAccountRequest(
    [Required] Guid SessionKey,
    [Required] string Number,
    [Required] string PinCode,
    [Required] string OwnerUserId,
    string? AccountType = null);
