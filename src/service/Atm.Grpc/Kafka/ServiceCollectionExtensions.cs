using Accounts.Kafka.Contracts;
using ApprovalResult.Kafka.Contracts;
using Atm.Application;
using Invoices.Kafka.Contracts;
using Itmo.Dev.Platform.Kafka.Configuration;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.MessagePersistence;
using Itmo.Dev.Platform.MessagePersistence.Postgres.Extensions;

namespace Atm.Grpc.Kafka;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAccountEventPublisher, AccountEventPublisher>();
        services.AddScoped<IInvoiceEventPublisher, InvoiceEventPublisher>();

        services.AddPlatformKafka(kafka => kafka
            .ConfigureOptions(configuration.GetSection("Presentation:Kafka"))
            .AddProducers(configuration)
            .AddConsumers(configuration));

        services.AddPlatformMessagePersistence(step => step
            .WithDefaultPublisherOptions("MessagePersistence:Publisher:Default")
            .UsePostgresPersistence(postgres => postgres.ConfigureOptions("MessagePersistence:Postgres")));

        return services;
    }

    private static IKafkaConfigurationBuilder AddProducers(this IKafkaConfigurationBuilder kafka, IConfiguration configuration)
    {
        IConfiguration producers = configuration.GetSection("Presentation:Kafka:Producers");

        kafka.AddProducer(b => b
            .WithKey<AccountCreationKey>()
            .WithValue<AccountCreationValue>()
            .WithConfiguration(producers.GetSection("AccountCreated"))
            .SerializeKeyWithProto()
            .SerializeValueWithProto()
            .WithOutbox());

        kafka.AddProducer(b => b
            .WithKey<InvoiceCreationKey>()
            .WithValue<InvoiceCreationValue>()
            .WithConfiguration(producers.GetSection("InvoiceCreated"))
            .SerializeKeyWithProto()
            .SerializeValueWithProto()
            .WithOutbox());

        return kafka;
    }

    private static IKafkaConfigurationBuilder AddConsumers(this IKafkaConfigurationBuilder kafka, IConfiguration configuration)
    {
        IConfiguration consumers = configuration.GetSection("Presentation:Kafka:Consumers");

        kafka.AddConsumer(b => b
            .WithKey<ApprovalResultKey>()
            .WithValue<ApprovalResultValue>()
            .WithConfiguration(consumers.GetSection("ApprovalResult"))
            .DeserializeKeyWithProto()
            .DeserializeValueWithProto()
            .HandleWith<ApprovalResultKafkaHandler>());

        return kafka;
    }
}
