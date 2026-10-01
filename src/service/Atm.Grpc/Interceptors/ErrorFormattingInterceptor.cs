using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Text.Json;

namespace Atm.Grpc.Interceptors;

public class ErrorFormattingInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException exception)
        {
            throw new RpcException(Format(exception.StatusCode, exception.Status.Detail));
        }
        catch (Exception)
        {
            throw new RpcException(Format(StatusCode.Internal, "Internal server error"));
        }
    }

    private static Status Format(StatusCode code, string message)
    {
        string detail = JsonSerializer.Serialize(new ErrorDetail(code.ToString(), message));
        return new Status(code, detail);
    }

    private sealed record ErrorDetail(string Code, string Message);
}
