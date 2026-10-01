namespace Atm.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class DatabaseCollection : ICollectionFixture<AtmDatabaseFixture>, ICollectionFixture<AtmGrpcServiceFixture>
{
    internal const string Name = "Database";
}
