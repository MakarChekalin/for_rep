using Atm.Client.Application;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Atm.Client.Console.Commands;

public class LoginUserSettings : CommandSettings // что нужно ввести
{
    [CommandArgument(0, "<accountNumber>")]
    public string AccountNumber { get; set; } = string.Empty;

    [CommandArgument(1, "<pinCode>")]
    public string PinCode { get; set; } = string.Empty;
}

public class LoginUserCommand : AsyncCommand<LoginUserSettings> // что сделать с этими введёнными данными
{
    private readonly AtmClientService _service;

    public LoginUserCommand(AtmClientService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, LoginUserSettings settings, CancellationToken cancellationToken) // CancellationToken для отмены(ctrl + c) 
    {
        var success = await _service.LoginUserAsync(settings.AccountNumber, settings.PinCode);

        if (success)
            AnsiConsole.MarkupLine("[green]Logged in successfully.[/]");
        else
            AnsiConsole.MarkupLine("[red]Login failed.[/]");

        return 0;
    }
}