using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pg.DataverseSync.Engine.Functions.ForceTableResync;

namespace Pg.DataverseSync.Engine.Functions.Tests.ForceTableResync;

[ExcludeFromCodeCoverage]
public class ForceTableResyncStatusFunctionTests
{
    private readonly ForceTableResyncStatusFunction _function =
        new(Substitute.For<ILogger<ForceTableResyncStatusFunction>>());

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ForceTableResyncStatusFunction(null!));
    }

    [Theory]
    [InlineData(OrchestrationRuntimeStatus.Running, true)]
    [InlineData(OrchestrationRuntimeStatus.Pending, true)]
    [InlineData(OrchestrationRuntimeStatus.Suspended, true)]
    [InlineData(OrchestrationRuntimeStatus.Completed, false)]
    [InlineData(OrchestrationRuntimeStatus.Failed, false)]
    [InlineData(OrchestrationRuntimeStatus.Terminated, false)]
    public async Task Run_ReturnsIsRunningBasedOnStatus(OrchestrationRuntimeStatus status, bool expectedIsRunning)
    {
        var (request, response) = HttpTestHelper.CreateRequest();
        var client = Substitute.For<DurableTaskClient>("test");
        var instanceId = ForceTableResyncInstanceId.For("account");
        client.GetInstanceAsync(instanceId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(new OrchestrationMetadata(nameof(ForceTableResyncOrchestrator), instanceId) { RuntimeStatus = status });

        var result = await _function.Run(request, client, "account");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        using var json = JsonDocument.Parse(response.ReadBody());
        Assert.Equal(expectedIsRunning, json.RootElement.GetProperty("isRunning").GetBoolean());
        Assert.Equal(status.ToString(), json.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Run_NoInstance_ReturnsNotStarted()
    {
        var (request, response) = HttpTestHelper.CreateRequest();
        var client = Substitute.For<DurableTaskClient>("test");
        client.GetInstanceAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns((OrchestrationMetadata?)null);

        await _function.Run(request, client, "account");

        using var json = JsonDocument.Parse(response.ReadBody());
        Assert.False(json.RootElement.GetProperty("isRunning").GetBoolean());
        Assert.Equal("NotStarted", json.RootElement.GetProperty("status").GetString());
    }
}
