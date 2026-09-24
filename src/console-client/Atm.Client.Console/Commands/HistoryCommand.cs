using Atm.Client.Application;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class HistoryCommand : AsyncCommand
{
    private readonly IAtmClientService _service;

    public HistoryCommand(IAtmClientService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var (success, history) = await _service.GetHistoryAsync();

        if (!success || history == null)
        {
            AnsiConsole.MarkupLine("[red]Failed to get history.[/]");
            return 0;
        }

        var table = new Table();
        table.AddColumn("Type");
        table.AddColumn("Amount");
        table.AddColumn("Timestamp");

        foreach (var op in history)
        {
            var typeName = op.Type == 0 ? "Withdraw" : "Deposit";
            table.AddRow(typeName, op.Amount.ToString(), op.Timestamp.ToString());
        }

        AnsiConsole.Write(table);

        return 0;
    }
}
