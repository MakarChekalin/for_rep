using Atm.Infrastructure;
using FluentMigrator.Runner;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddPersistence();
builder.Services.AddApplication();

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope()) // применение миграций при страрте
{
    IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

app.Run();
