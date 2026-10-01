using Atm.Domain;
using Grpc.Core;
using Users.Grpc.Contracts;

namespace Atm.Grpc.Services;

public class UserGrpcService : UserService.UserServiceBase
{
    private readonly IUserRepository _userRepository;

    public UserGrpcService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override async Task<GetUserByIdResponse> IsExists(GetUserByIdRequest request, ServerCallContext context)
    {
        bool exists = await _userRepository.ExistsByExternalIdAsync(request.UserId);

        return new GetUserByIdResponse { IsExists = exists };
    }
}
