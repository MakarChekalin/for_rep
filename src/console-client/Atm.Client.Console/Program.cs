using Atm.Client.Application;
using Atm.Client.Console.Commands;
using Atm.Client.Domain;
using Atm.Client.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Spectre.Console;
using Spectre.Console.Cli;

var services = new ServiceCollection();

services.AddRefitClient<IAtmApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5151"));

services.AddSingleton<IAtmGateway, RefitAtmGateway>();
services.AddSingleton<ClientSessionState>();
services.AddSingleton<AtmClientService>();

var provider = services.BuildServiceProvider(); // собираем контейнер сами
var clientService = provider.GetRequiredService<AtmClientService>(); // один готовый сервис для всех команд

var registrar = new Atm.Client.Console.TypeRegistrar(services); // мост между DI и Spectre.Console.сli

var sessionState = provider.GetRequiredService<ClientSessionState>(); // один обьект на все команды
registrar.RegisterInstance(typeof(ClientSessionState), sessionState); // явно говорим используй именно этот
registrar.RegisterInstance(typeof(AtmClientService), clientService); // чтобы сессия не терялась между командами

// создаем команды вручную и регистрируем как готовые объекты
// так надежнее чем полагаться на автосоздание через DI
registrar.RegisterInstance(typeof(LoginUserCommand), new LoginUserCommand(clientService));
registrar.RegisterInstance(typeof(LoginAdminCommand), new LoginAdminCommand(clientService));
registrar.RegisterInstance(typeof(CreateAccountCommand), new CreateAccountCommand(clientService));
registrar.RegisterInstance(typeof(WithdrawCommand), new WithdrawCommand(clientService));
registrar.RegisterInstance(typeof(DepositCommand), new DepositCommand(clientService));
registrar.RegisterInstance(typeof(BalanceCommand), new BalanceCommand(clientService));
registrar.RegisterInstance(typeof(HistoryCommand), new HistoryCommand(clientService));

var app = new CommandApp(registrar); // создает объекты команд из Commands

app.Configure(config => // список наших команд
{
    config.AddCommand<LoginUserCommand>("login-user");
    config.AddCommand<LoginAdminCommand>("login-admin");
    config.AddCommand<CreateAccountCommand>("create-account");
    config.AddCommand<WithdrawCommand>("withdraw");
    config.AddCommand<DepositCommand>("deposit");
    config.AddCommand<BalanceCommand>("balance");
    config.AddCommand<HistoryCommand>("history");
});

AnsiConsole.MarkupLine("Hi!");
AnsiConsole.MarkupLine("Commands: login-user, login-admin, create-account, withdraw, deposit, balance, history, exit");

while (true)
{
    var input = AnsiConsole.Ask<string>(">"); // читка(метод из Spectre.consol)

    if (string.Equals(input.Trim(), "exit", StringComparison.OrdinalIgnoreCase))
        break;

    var commandArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries); // разбиение на слова (просто args уже занято )

    await app.RunAsync(commandArgs);
}