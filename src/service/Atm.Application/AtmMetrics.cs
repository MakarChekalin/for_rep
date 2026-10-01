using System.Diagnostics.Metrics;

namespace Atm.Application;

public static class AtmMetrics
{
    public static Counter<long> AccountsCreated { get; } = AtmTelemetry.Meter.CreateCounter<long>("atm.accounts.created");

    public static Counter<long> Withdrawals { get; } = AtmTelemetry.Meter.CreateCounter<long>("atm.withdrawals");

    public static Counter<long> Deposits { get; } = AtmTelemetry.Meter.CreateCounter<long>("atm.deposits");

    public static Counter<long> InvoicesCreated { get; } = AtmTelemetry.Meter.CreateCounter<long>("atm.invoices.created");

    public static Counter<long> InvoicesPaid { get; } = AtmTelemetry.Meter.CreateCounter<long>("atm.invoices.paid");

    public static Counter<long> InvoicesCancelled { get; } = AtmTelemetry.Meter.CreateCounter<long>("atm.invoices.cancelled");
}
