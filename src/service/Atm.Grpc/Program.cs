using Atm.Application;
using Atm.Grpc.Integrations;
using Atm.Grpc.Interceptors;
using Atm.Grpc.Kafka;
using Atm.Grpc.Services;
using Atm.Infrastructure;
using Atm.ServiceDefaults;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddPersistence();
builder.Services.AddApplication();
builder.Services.AddKafkaMessaging(builder.Configuration);
builder.Services.AddInvoiceApprovalIntegration();

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
app.MapGrpcService<UserGrpcService>();

if (app.Environment.IsDevelopment())
    app.MapGrpcReflectionService();

app.Run();

public partial class Program;
