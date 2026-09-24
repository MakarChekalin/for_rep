using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class LoginAdminSettings : CommandSettings
{
    [CommandArgument(0, "<password>")]
    public string Password { get; set; } = string.Empty;
}
