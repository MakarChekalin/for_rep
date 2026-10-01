using Atm.Application;
using Atm.Grpc.Interceptors;
using Atm.Grpc.Services;
using Atm.Infrastructure;
using Atm.ServiceDefaults;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddPersistence();
builder.Services.AddApplication();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<TimingInterceptor>();
    options.Interceptors.Add<ErrorFormattingInterceptor>();
    options.Interceptors.Add<UserProvisioningInterceptor>();
});
builder.Services.AddGrpcReflection();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.MapGrpcService<SessionGrpcService>();
app.MapGrpcService<AccountGrpcService>();
app.MapGrpcService<InvoiceGrpcService>();

if (app.Environment.IsDevelopment())
    app.MapGrpcReflectionService();

app.Run();
