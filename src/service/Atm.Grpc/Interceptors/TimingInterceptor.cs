using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Diagnostics;

namespace Atm.Grpc.Interceptors;

public class TimingInterceptor : Interceptor
{
    private readonly ILogger<TimingInterceptor> _logger;

    public TimingInterceptor(ILogger<TimingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            return await continuation(request, context);
        }
        finally
        {
            stopwatch.Stop();
            TimingInterceptorLog.MethodCompleted(_logger, context.Method, stopwatch.ElapsedMilliseconds);
        }
    }
}
