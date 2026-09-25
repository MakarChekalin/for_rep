using Atm.Client.Application;
using Atm.Client.Domain;
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
        WithdrawResult result = await _service.WithdrawAsync(settings.Amount);

        switch (result)
        {
            case WithdrawResult.Success:
                AnsiConsole.MarkupLine("[green]Withdrawal successful.[/]");
                break;
            case WithdrawResult.NotLoggedIn:
                AnsiConsole.MarkupLine("[red]Not logged in.[/]");
                break;
            default:
                AnsiConsole.MarkupLine("[red]Withdrawal failed.[/]");
                break;
        }

        return 0;
    }
}
