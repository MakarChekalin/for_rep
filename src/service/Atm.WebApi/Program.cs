using Atm.Application;
using Atm.Domain;
using Atm.Infrastructure;
using Atm.Infrastructure.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<DatabaseOptions>(options => // теперь в конструкторах будет понятно откуда брать IOptions<DatabaseOptions>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("Postgres");
});

builder.Services.AddSingleton<IAccountRepository, PostgresAccountRepository>();
builder.Services.AddSingleton<ISessionRepository, PostgresSessionRepository>();
builder.Services.AddSingleton<IOperationRepository, PostgresOperationRepository>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<SessionService>(sp =>
    new SessionService(
        sp.GetRequiredService<ISessionRepository>(),
        sp.GetRequiredService<IAccountRepository>(),
        builder.Configuration["AdminPassword"] ?? "admin123"));

builder.Services.AddFluentMigratorCore() // настройка миграции(куда подключаться)
    .ConfigureRunner(rb => rb
        .AddPostgres() // что за бд
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("Postgres")) // куда подкл.
        .ScanIn(typeof(Migration001_InitialSchema).Assembly).For.Migrations()); // тут ищем все классы миграции 

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