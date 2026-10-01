using System.ComponentModel.DataAnnotations;

namespace Atm.Gateway.DTO;

public record InvoiceActionRequest(
    [Required] Guid SessionKey);
