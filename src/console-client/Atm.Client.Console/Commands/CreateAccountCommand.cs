using Atm.Client.Application;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Atm.Client.Console.Commands;

public class CreateAccountCommand : AsyncCommand<CreateAccountSettings>
{
    private readonly IAtmClientService _service;

    public CreateAccountCommand(IAtmClientService service)
    {
        _service = service;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, CreateAccountSettings settings, CancellationToken cancellationToken)
    {
        Domain.CreateAccountResult result = await _service.CreateAccountAsync(settings.Number, settings.PinCode);

        if (result == Domain.CreateAccountResult.Success)
            AnsiConsole.MarkupLine("[green]Account created.[/]");
        else
            AnsiConsole.MarkupLine($"[red]Create account failed.[/]");

        return 0;
    }
}