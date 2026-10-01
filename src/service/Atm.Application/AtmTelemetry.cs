using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Atm.Application;

public static class AtmTelemetry
{
    private const string Name = "Atm.Application";

    public static ActivitySource ActivitySource { get; } = new(Name);

    public static Meter Meter { get; } = new(Name);
}
