using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class CreateAccountSettings : CommandSettings
{
    [CommandArgument(0, "<number>")]
    public string Number { get; set; } = string.Empty;

    [CommandArgument(1, "<pinCode>")]
    public string PinCode { get; set; } = string.Empty;
}
