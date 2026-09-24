using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class AmountSettings : CommandSettings
{
    [CommandArgument(0, "<amount>")]
    public decimal Amount { get; set; }
}
