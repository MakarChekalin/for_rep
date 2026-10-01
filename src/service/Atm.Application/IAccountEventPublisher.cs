using Atm.Domain;

namespace Atm.Application;

public interface IAccountEventPublisher
{
    Task PublishAccountCreatedAsync(long ownerExternalId, long accountExternalId, AccountType type, CancellationToken cancellationToken);
}
