using Accounts.Kafka.Contracts;
using Atm.Application;
using Itmo.Dev.Platform.Kafka.Producer;

namespace Atm.Grpc.Kafka;

public class AccountEventPublisher : IAccountEventPublisher
{
    private readonly IKafkaMessageProducer<AccountCreationKey, AccountCreationValue> _producer;

    public AccountEventPublisher(IKafkaMessageProducer<AccountCreationKey, AccountCreationValue> producer)
    {
        _producer = producer;
    }

    public async Task PublishAccountCreatedAsync(long ownerExternalId, long accountExternalId, Atm.Domain.AccountType type, CancellationToken cancellationToken)
    {
        var key = new AccountCreationKey { AccountId = accountExternalId };

        var value = new AccountCreationValue
        {
            UserId = ownerExternalId,
            AccountId = accountExternalId,
            AccountType = ToProto(type),
        };

        var message = new KafkaProducerMessage<AccountCreationKey, AccountCreationValue>(Key: key, Value: value);

        await _producer.ProduceAsync(ToAsyncEnumerable(message), cancellationToken);
    }

    private static Accounts.Kafka.Contracts.AccountType ToProto(Atm.Domain.AccountType type) => type switch
    {
        Atm.Domain.AccountType.Personal => Accounts.Kafka.Contracts.AccountType.Personal,
        Atm.Domain.AccountType.Corporate => Accounts.Kafka.Contracts.AccountType.Corporate,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    private static async IAsyncEnumerable<KafkaProducerMessage<AccountCreationKey, AccountCreationValue>> ToAsyncEnumerable(
        KafkaProducerMessage<AccountCreationKey, AccountCreationValue> message)
    {
        yield return message;
        await Task.CompletedTask;
    }
}
