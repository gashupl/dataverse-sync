var builder = DistributedApplication.CreateBuilder(args);

const int webAppPort = 5173;

var webAppWorkingDirectory = Path.GetFullPath(
    Path.Combine(builder.AppHostDirectory, "../../../client/Pg.DataverseSync.Web"));

//See ADR-0002: docs/adr/0002-db-container-auto-generation-resign.md
var database = builder.AddConnectionString("sqldb");

var webApp = builder.AddExecutable("pg-dataversesync-web", "npm", webAppWorkingDirectory, "run", "dev")
    .WithHttpEndpoint(port: webAppPort, env: "PORT")
    .WithExternalHttpEndpoints();

var api = builder.AddProject<Projects.Pg_DataverseSync_Api>("pg-dataversesync-api")
    .WithReference(database)
    .WithEnvironment("AllowedOrigins__0", $"http://localhost:{webAppPort.ToString()}")
    .WithEnvironment("AllowedOrigins__1", $"https://localhost:{webAppPort.ToString()}"); // Support both HTTP and HTTPS

webApp.WithReference(api)
    .WaitFor(api);

builder.Build().Run();
