using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class LoginUserSettings : CommandSettings // что нужно ввести
{
    [CommandArgument(0, "<accountNumber>")]
    public string AccountNumber { get; set; } = string.Empty;

    [CommandArgument(1, "<pinCode>")]
    public string PinCode { get; set; } = string.Empty;
}
