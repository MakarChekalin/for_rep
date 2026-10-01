namespace Atm.IntegrationTests;

[Collection(DatabaseCollection.Name)]
public abstract class GrpcServiceTestBase
{
    protected GrpcServiceTestBase(AtmGrpcServiceFixture grpc)
    {
        Grpc = grpc;
    }

    protected AtmGrpcServiceFixture Grpc { get; }
}
