using System.ComponentModel.DataAnnotations;

namespace Atm.Gateway.DTO;

public record CreateInvoiceRequest(
    [Required] Guid SessionKey,
    [Required] string PayerAccountNumber,
    [Range(0.01, double.MaxValue)] decimal Amount);
