using Grpc.Core;
using System.Text.Json;

namespace Atm.Gateway;

public class GrpcExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GrpcExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (RpcException exception)
        {
            context.Response.StatusCode = ToHttpStatusCode(exception.StatusCode);
            context.Response.ContentType = "application/json";

            string detail = exception.Status.Detail;

            if (IsJson(detail))
                await context.Response.WriteAsync(detail);
            else
                await context.Response.WriteAsJsonAsync(new { error = detail });
        }
    }

    private static bool IsJson(string value)
    {
        try
        {
            using var document = JsonDocument.Parse(value);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static int ToHttpStatusCode(StatusCode statusCode) => statusCode switch
    {
        StatusCode.OK => StatusCodes.Status200OK,
        StatusCode.Cancelled => 499,
        StatusCode.Unknown => StatusCodes.Status500InternalServerError,
        StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
        StatusCode.DeadlineExceeded => StatusCodes.Status504GatewayTimeout,
        StatusCode.NotFound => StatusCodes.Status404NotFound,
        StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
        StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
        StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
        StatusCode.ResourceExhausted => StatusCodes.Status429TooManyRequests,
        StatusCode.FailedPrecondition => StatusCodes.Status409Conflict,
        StatusCode.Aborted => StatusCodes.Status409Conflict,
        StatusCode.OutOfRange => StatusCodes.Status400BadRequest,
        StatusCode.Unimplemented => StatusCodes.Status501NotImplemented,
        StatusCode.Internal => StatusCodes.Status500InternalServerError,
        StatusCode.Unavailable => StatusCodes.Status503ServiceUnavailable,
        StatusCode.DataLoss => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError,
    };
}
