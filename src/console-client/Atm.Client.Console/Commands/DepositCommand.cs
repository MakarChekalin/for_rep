using Atm.Client.Application;
using Atm.Client.Domain;
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
        DepositResult result = await _service.DepositAsync(settings.Amount);

        switch (result)
        {
            case DepositResult.Success:
                AnsiConsole.MarkupLine("[green]Deposit successful.[/]");
                break;
            case DepositResult.NotLoggedIn:
                AnsiConsole.MarkupLine("[red]Not logged in.[/]");
                break;
            default:
                AnsiConsole.MarkupLine("[red]Deposit failed.[/]");
                break;
        }

        return 0;
    }
}
