namespace Atm.Client.Infrastructure;

public record AmountBody(Guid SessionKey, decimal Amount);
