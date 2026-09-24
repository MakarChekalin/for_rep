using Atm.Client.Application;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class WithdrawCommand : AsyncCommand<AmountSettings>
{
    private readonly IAtmClientService _service;

    public WithdrawCommand(IAtmClientService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, AmountSettings settings, CancellationToken cancellationToken)
    {
        var (success, error) = await _service.WithdrawAsync(settings.Amount);

        if (success)
            AnsiConsole.MarkupLine("[green]Withdrawal successful.[/]");
        else
            AnsiConsole.MarkupLine($"[red]Failed: {error}[/]");

        return 0;
    }
}