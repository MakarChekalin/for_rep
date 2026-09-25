using Atm.Application;
using Atm.Domain;
using Atm.Infrastructure;
using Atm.Infrastructure.Migrations;
using Atm.WebApi;
using FluentMigrator.Runner;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// конфигурации читаются из IConfiguration и отдаются через IOptions с валидацией при старте
builder.Services.AddOptions<DatabaseOptions>()
    .Configure<IConfiguration>((options, configuration) =>
        options.ConnectionString = configuration.GetConnectionString("Postgres") ?? string.Empty)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<AdminOptions>()
    .Configure<IConfiguration>((options, configuration) =>
        options.Password = configuration["AdminPassword"] ?? string.Empty)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IAccountRepository, PostgresAccountRepository>();
builder.Services.AddSingleton<ISessionRepository, PostgresSessionRepository>();
builder.Services.AddSingleton<IOperationRepository, PostgresOperationRepository>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton(serviceProvider =>
    new SessionService(
        serviceProvider.GetRequiredService<ISessionRepository>(),
        serviceProvider.GetRequiredService<IAccountRepository>(),
        serviceProvider.GetRequiredService<IOptions<AdminOptions>>().Value.Password));

builder.Services.AddFluentMigratorCore() // настройка миграции(куда подключаться)
    .ConfigureRunner(rb => rb
        .AddPostgres() // что за бд
        .WithGlobalConnectionString(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString) // куда подкл.
        .ScanIn(typeof(Migration001InitialSchema).Assembly).For.Migrations()); // тут ищем все классы миграции

var app = builder.Build();

using (var scope = app.Services.CreateScope()) // применение миграций при страрте
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

app.Run();