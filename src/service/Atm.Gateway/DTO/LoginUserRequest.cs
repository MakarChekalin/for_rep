using System.ComponentModel.DataAnnotations;

namespace Atm.Gateway.DTO;

public record LoginUserRequest(
    [Required] string AccountNumber,
    [Required] string PinCode);
