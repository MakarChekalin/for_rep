using Atm.Application;
using Atm.Domain;
using Atm.Infrastructure;
using Atm.Infrastructure.Migrations;
using FluentMigrator.Runner;
using Npgsql;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

string connectionString = builder.Configuration.GetConnectionString("Postgres")
?? throw new InvalidOperationException("Connection string not found!!!");

builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString));
builder.Services.AddSingleton<IAccountRepository>(_ => new PostgresAccountRepository(connectionString));
builder.Services.AddSingleton<ISessionRepository>(_ => new PostgresSessionRepository(connectionString));
builder.Services.AddSingleton<IOperationRepository>(_ => new PostgresOperationRepository(connectionString));
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton(_ =>
    new SessionService(
        _.GetRequiredService<ISessionRepository>(),
        _.GetRequiredService<IAccountRepository>(),
        builder.Configuration["AdminPassword"] ?? "admin123"));

builder.Services.AddFluentMigratorCore() // настройка миграции(куда подключаться)
    .ConfigureRunner(rb => rb
        .AddPostgres() // что за бд
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("Postgres")) // куда подкл.
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