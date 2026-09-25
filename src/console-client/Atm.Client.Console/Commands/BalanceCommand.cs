using Atm.Client.Application;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class BalanceCommand : AsyncCommand
{
    private readonly IAtmClientService _service;

    public BalanceCommand(IAtmClientService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        decimal? balance = await _service.GetBalanceAsync();

        if (balance != null)
            AnsiConsole.MarkupLine($"[green]Balance: {balance}[/]");
        else
            AnsiConsole.MarkupLine("[red]Failed to get balance.[/]");

        return 0;
    }
}
