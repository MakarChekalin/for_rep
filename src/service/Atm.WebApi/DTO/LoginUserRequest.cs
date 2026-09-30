using System.ComponentModel.DataAnnotations;

namespace Atm.WebApi.DTO;

public record LoginUserRequest(
    [Required] string AccountNumber,
    [Required] string PinCode);