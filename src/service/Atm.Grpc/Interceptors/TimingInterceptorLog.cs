namespace Atm.Grpc.Interceptors;

internal static partial class TimingInterceptorLog
{
    [LoggerMessage(Level = LogLevel.Information, Message = "{Method} completed in {ElapsedMilliseconds} ms")]
    public static partial void MethodCompleted(ILogger logger, string method, long elapsedMilliseconds);
}
