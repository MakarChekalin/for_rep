using Google.Type;

namespace Atm.Grpc.Kafka;

internal static class MoneyMapping
{
    private const string CurrencyCode = "RUB";

    public static Money ToMoney(decimal amount)
    {
        long units = (long)amount;
        int nanos = (int)((amount - units) * 1_000_000_000m);

        return new Money
        {
            CurrencyCode = CurrencyCode,
            Units = units,
            Nanos = nanos,
        };
    }
}
