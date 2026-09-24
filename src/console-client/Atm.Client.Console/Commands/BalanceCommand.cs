using Atm.Client.Application;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class BalanceCommand : AsyncCommand
{
    private readonly AtmClientService _service;

    public BalanceCommand(AtmClientService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var (success, balance) = await _service.GetBalanceAsync();

        if (success)
            AnsiConsole.MarkupLine($"[green]Balance: {balance}[/]");
        else
            AnsiConsole.MarkupLine("[red]Failed to get balance.[/]");

        return 0;
    }
}
