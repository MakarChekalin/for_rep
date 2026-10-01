namespace Atm.Application;

public interface IAdminPasswordValidator
{
    bool IsValid(string password);
}