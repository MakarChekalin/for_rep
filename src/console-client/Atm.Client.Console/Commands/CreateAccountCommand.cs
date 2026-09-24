using Atm.Client.Application;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class CreateAccountCommand : AsyncCommand<CreateAccountSettings>
{
    private readonly AtmClientService _service;

    public CreateAccountCommand(AtmClientService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, CreateAccountSettings settings, CancellationToken cancellationToken)
    {
        var (success, error) = await _service.CreateAccountAsync(settings.Number, settings.PinCode);

        if (success)
            AnsiConsole.MarkupLine("[green]Account created.[/]");
        else
            AnsiConsole.MarkupLine($"[red]Failed: {error}[/]");

        return 0;
    }
}