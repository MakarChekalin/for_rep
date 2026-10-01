using Atm.Application;
using Atm.Grpc.Interceptors;
using Atm.Grpc.Services;
using Atm.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence();
builder.Services.AddApplication();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<TimingInterceptor>();
    options.Interceptors.Add<ErrorFormattingInterceptor>();
});
builder.Services.AddGrpcReflection();

WebApplication app = builder.Build();

app.MapGrpcService<SessionGrpcService>();
app.MapGrpcService<AccountGrpcService>();
app.MapGrpcService<InvoiceGrpcService>();

if (app.Environment.IsDevelopment())
    app.MapGrpcReflectionService();

app.Run();
