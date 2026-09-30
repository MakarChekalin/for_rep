using Atm.Client.Application;
using Atm.Client.Application.DTO;
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
        List<OperationDto>? history = await _service.GetHistoryAsync();

        if (history == null)
        {
            AnsiConsole.MarkupLine("[red]Failed to get history.[/]");
            return 0;
        }

        var table = new Table();
        table.AddColumn("Type");
        table.AddColumn("Amount");
        table.AddColumn("Timestamp");

        foreach (OperationDto op in history)
        {
            string typeName = op.Type == 0 ? "Withdraw" : "Deposit";
            table.AddRow(typeName, op.Amount.ToString(), op.Timestamp.ToString());
        }

        AnsiConsole.Write(table);

        return 0;
    }
}
