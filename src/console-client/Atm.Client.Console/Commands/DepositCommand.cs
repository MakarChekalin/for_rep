using Atm.Client.Application;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

// не сделал отдельный класс settings(уже есть в WithdrawCommand)
public class DepositCommand : AsyncCommand<AmountSettings>
{
    private readonly IAtmClientService _service;

    public DepositCommand(IAtmClientService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, AmountSettings settings, CancellationToken cancellationToken)
    {
        var (success, error) = await _service.DepositAsync(settings.Amount);

        if (success)
            AnsiConsole.MarkupLine("[green]Deposit successful.[/]");
        else
            AnsiConsole.MarkupLine($"[red]Failed: {error}[/]");

        return 0;
    }
}