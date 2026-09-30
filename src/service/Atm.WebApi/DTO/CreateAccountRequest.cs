using System.ComponentModel.DataAnnotations;

namespace Atm.WebApi.DTO;

public record CreateAccountRequest(
    [Required] Guid SessionKey,
    [Required] string Number,
    [Required] string PinCode);
