var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Pg_DataverseSync_Api>("pg-dataversesync-api");

builder.Build().Run();
