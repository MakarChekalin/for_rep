namespace Atm.Client.Infrastructure;

public record CreateAccountBody(Guid SessionKey, string Number, string PinCode);
