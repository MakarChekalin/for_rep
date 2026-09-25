using Atm.Client.Application;
using Atm.Client.Domain;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class LoginAdminCommand : AsyncCommand<LoginAdminSettings>
{
    private readonly IAtmClientService _service;

    public LoginAdminCommand(IAtmClientService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, LoginAdminSettings settings, CancellationToken cancellationToken)
    {
        LoginResult result = await _service.LoginAdminAsync(settings.Password);

        if (result == LoginResult.Success)
            AnsiConsole.MarkupLine("[green]Logged in as admin.[/]");
        else
            AnsiConsole.MarkupLine("[red]Login failed.[/]");

        return 0;
    }
}