using Atm.Application;
using Atm.Grpc.Services;
using Atm.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence();
builder.Services.AddApplication();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

WebApplication app = builder.Build();

app.MapGrpcService<SessionGrpcService>();
app.MapGrpcService<AccountGrpcService>();
app.MapGrpcService<InvoiceGrpcService>();

if (app.Environment.IsDevelopment())
    app.MapGrpcReflectionService();

app.Run();
