using Atm.Client.Application;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class LoginAdminSettings : CommandSettings
{
    [CommandArgument(0, "<password>")]
    public string Password { get; set; } = string.Empty;
}

public class LoginAdminCommand : AsyncCommand<LoginAdminSettings>
{
    private readonly AtmClientService _service;

    public LoginAdminCommand(AtmClientService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, LoginAdminSettings settings, CancellationToken cancellationToken)
    {
        var success = await _service.LoginAdminAsync(settings.Password);

        if (success)
            AnsiConsole.MarkupLine("[green]Logged in as admin.[/]");
        else
            AnsiConsole.MarkupLine("[red]Login failed.[/]");

        return 0;
    }
}