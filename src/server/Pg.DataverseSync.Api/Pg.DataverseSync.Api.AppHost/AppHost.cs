var builder = DistributedApplication.CreateBuilder(args);

var webAppWorkingDirectory = Path.GetFullPath(
    Path.Combine(builder.AppHostDirectory, "../../../client/Pg.DataverseSync.Web"));

//See ADR-0002: docs/adr/0002-db-container-auto-generation-resign.md
var database = builder.AddConnectionString("sqldb");

var api = builder.AddProject<Projects.Pg_DataverseSync_Api>("pg-dataversesync-api")
    .WithReference(database);

//// Diagnostic: Print the resolved path
//builder.AddExecutable("print-path", "cmd", builder.AppHostDirectory, "/c", "echo", webAppWorkingDirectory);

//// Diagnostic: Check if working directory is valid
//builder.AddExecutable("check-web-directory", "cmd", webAppWorkingDirectory, "/c", "dir");

builder.AddExecutable("pg-dataversesync-web", "npm", webAppWorkingDirectory, "run", "dev")
    .WithHttpEndpoint(port: 5173, env: "PORT")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
