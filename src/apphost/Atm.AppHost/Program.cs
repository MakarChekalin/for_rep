IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> postgresPassword = builder.AddParameter("postgres-password", "postgres");

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("atm3-postgres", password: postgresPassword, port: 5434)
    .WithEndpoint("tcp", endpoint => endpoint.IsProxied = false)
    .WithDataVolume("atm3-postgres-data");

IResourceBuilder<PostgresDatabaseResource> atmDatabase = postgres.AddDatabase("atmdb3");

IResourceBuilder<ContainerResource> keycloak = builder
    .AddContainer("atm3-keycloak", "quay.io/keycloak/keycloak", "26.0")
    .WithEndpoint(port: 8081, targetPort: 8080, name: "http", isProxied: false)
    .WithEnvironment("KEYCLOAK_ADMIN", "admin")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "admin")
    .WithBindMount("../../../keycloak/realm-export.json", "/opt/keycloak/data/import/realm.json")
    .WithArgs("start-dev", "--import-realm");

IResourceBuilder<ProjectResource> grpcService = builder
    .AddProject<Projects.Atm_Grpc>("atm-grpc")
    .WaitFor(atmDatabase)
    .WaitFor(keycloak);

builder
    .AddProject<Projects.Atm_Gateway>("atm-gateway")
    .WaitFor(grpcService)
    .WaitFor(keycloak);

builder.Build().Run();
