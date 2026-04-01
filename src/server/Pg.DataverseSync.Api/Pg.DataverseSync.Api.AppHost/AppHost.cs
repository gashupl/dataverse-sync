var builder = DistributedApplication.CreateBuilder(args);

//See ADR-0002: docs/adr/0002-db-container-auto-generation-resign.md
var database = builder.AddConnectionString("sqldb");

builder.AddProject<Projects.Pg_DataverseSync_Api>("pg-dataversesync-api")
    .WithReference(database);

builder.Build().Run();
