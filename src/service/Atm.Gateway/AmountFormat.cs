using System.Globalization;

namespace Atm.Gateway;

internal static class AmountFormat
{
    public static string ToGrpc(decimal amount)
    {
        return amount.ToString(CultureInfo.InvariantCulture);
    }

    public static decimal FromGrpc(string amount)
    {
        return decimal.Parse(amount, CultureInfo.InvariantCulture);
    }
}
