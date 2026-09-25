using Atm.Client.Application;
using Atm.Client.Domain;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class LoginUserCommand : AsyncCommand<LoginUserSettings> // что сделать с этими введёнными данными
{
    private readonly IAtmClientService _service;

    public LoginUserCommand(IAtmClientService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, LoginUserSettings settings, CancellationToken cancellationToken) // CancellationToken для отмены(ctrl + c)
    {
        LoginResult result = await _service.LoginUserAsync(settings.AccountNumber, settings.PinCode);

        if (result == LoginResult.Success)
            AnsiConsole.MarkupLine("[green]Logged in successfully.[/]");
        else
            AnsiConsole.MarkupLine("[red]Login failed.[/]");

        return 0;
    }
}