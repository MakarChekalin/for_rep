namespace Atm.Application.Results;

public record GetBalanceResult(GetBalanceStatus Status, decimal Balance);
