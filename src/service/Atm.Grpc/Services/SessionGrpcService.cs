using Atm.Application;
using Grpc.Core;

namespace Atm.Grpc.Services;

public class SessionGrpcService : SessionService.SessionServiceBase
{
    private readonly ISessionService _sessionService;

    public SessionGrpcService(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public override async Task<LoginResponse> LoginUser(LoginUserRequest request, ServerCallContext context)
    {
        Guid? sessionKey = await _sessionService.LoginUserAsync(request.AccountNumber, request.PinCode);

        if (sessionKey == null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid account number or pin code"));

        return new LoginResponse { SessionKey = sessionKey.Value.ToString() };
    }

    public override async Task<LoginResponse> LoginAdmin(LoginAdminRequest request, ServerCallContext context)
    {
        Guid? sessionKey = await _sessionService.LoginAdminAsync(request.Password);

        if (sessionKey == null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid password"));

        return new LoginResponse { SessionKey = sessionKey.Value.ToString() };
    }
}
