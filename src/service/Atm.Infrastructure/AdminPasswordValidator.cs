using Atm.Application;

using Microsoft.Extensions.Options;

namespace Atm.Infrastructure;

public class AdminPasswordValidator : IAdminPasswordValidator
{
    private readonly string _password;

    public AdminPasswordValidator(IOptions<AdminOptions> options)
    {
        _password = options.Value.Password;
    }

    public bool IsValid(string password)
    {
        return password == _password;
    }
}