using Atm.Application;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Atm.Grpc.Interceptors;

public class UserProvisioningInterceptor : Interceptor
{
    private readonly IUserProvisioningService _userProvisioningService;

    public UserProvisioningInterceptor(IUserProvisioningService userProvisioningService)
    {
        _userProvisioningService = userProvisioningService;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        if (request is IUserIdentifiedRequest { UserId.Length: > 0 } identified)
            await _userProvisioningService.EnsureUserAsync(identified.UserId);

        return await continuation(request, context);
    }
}
