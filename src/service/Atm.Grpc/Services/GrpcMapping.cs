using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Globalization;

namespace Atm.Grpc.Services;

internal static class GrpcMapping
{
    public static string FormatAmount(decimal amount)
    {
        return amount.ToString(CultureInfo.InvariantCulture);
    }

    public static decimal ParseAmount(string amount, string fieldName)
    {
        if (!decimal.TryParse(amount, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result))
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"'{fieldName}' is not a valid amount"));

        return result;
    }

    public static Guid ParseGuid(string value, string fieldName)
    {
        if (!Guid.TryParse(value, out Guid result))
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"'{fieldName}' is not a valid identifier"));

        return result;
    }

    public static Timestamp ToTimestamp(DateTime value)
    {
        return Timestamp.FromDateTime(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }
}
