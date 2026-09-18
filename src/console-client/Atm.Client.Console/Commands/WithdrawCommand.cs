using Atm.Client.Application;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class AmountSettings : CommandSettings
{
    [CommandArgument(0, "<amount>")]
    public decimal Amount { get; set; }
}

public class WithdrawCommand : AsyncCommand<AmountSettings>
{
    private readonly AtmClientService _service;

    public WithdrawCommand(AtmClientService service)
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