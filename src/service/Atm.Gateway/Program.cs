using Atm.Gateway;
using Atm.Grpc;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddOptions<GrpcOptions>()
    .BindConfiguration("Grpc")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton(serviceProvider =>
    GrpcChannel.ForAddress(serviceProvider.GetRequiredService<IOptions<GrpcOptions>>().Value.Address));

builder.Services.AddSingleton(serviceProvider => new SessionService.SessionServiceClient(serviceProvider.GetRequiredService<GrpcChannel>()));
builder.Services.AddSingleton(serviceProvider => new AccountService.AccountServiceClient(serviceProvider.GetRequiredService<GrpcChannel>()));
builder.Services.AddSingleton(serviceProvider => new InvoiceService.InvoiceServiceClient(serviceProvider.GetRequiredService<GrpcChannel>()));

WebApplication app = builder.Build();

app.UseMiddleware<GrpcExceptionMiddleware>();

app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

app.Run();
