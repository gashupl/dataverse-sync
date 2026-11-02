using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pg.DataverseSync.Engine.Functions.ForceTableResync;

namespace Pg.DataverseSync.Engine.Functions.Tests.ForceTableResync;

[ExcludeFromCodeCoverage]
public class ForceTableResyncHttpStarterTests
{
    private readonly ForceTableResyncHttpStarter _starter =
        new(Substitute.For<ILogger<ForceTableResyncHttpStarter>>());

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ForceTableResyncHttpStarter(null!));
    }

    [Theory]
    [InlineData(OrchestrationRuntimeStatus.Running)]
    [InlineData(OrchestrationRuntimeStatus.Pending)]
    [InlineData(OrchestrationRuntimeStatus.Suspended)]
    public async Task Run_InstanceActive_ReturnsConflictAndDoesNotSchedule(OrchestrationRuntimeStatus status)
    {
        var (request, response) = HttpTestHelper.CreateRequest();
        var client = Substitute.For<DurableTaskClient>("test");
        var instanceId = ForceTableResyncInstanceId.For("account");
        client.GetInstanceAsync(instanceId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(new OrchestrationMetadata(nameof(ForceTableResyncOrchestrator), instanceId) { RuntimeStatus = status });

        var result = await _starter.Run(request, client, "account");

        Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        Assert.Contains("already running", response.ReadBody());
        await client.DidNotReceiveWithAnyArgs().ScheduleNewOrchestrationInstanceAsync(
            default, default, default, default);
    }

    [Fact]
    public async Task Run_NoInstance_SchedulesOrchestrationWithDeterministicInstanceId()
    {
        var (request, _) = HttpTestHelper.CreateRequest();
        var client = Substitute.For<DurableTaskClient>("test");
        var instanceId = ForceTableResyncInstanceId.For("account");
        client.GetInstanceAsync(instanceId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns((OrchestrationMetadata?)null);
        client.ScheduleNewOrchestrationInstanceAsync(
                Arg.Any<TaskName>(), Arg.Any<object?>(), Arg.Any<StartOrchestrationOptions?>(), Arg.Any<CancellationToken>())
            .Returns(instanceId);

        try
        {
            await _starter.Run(request, client, "account");
        }
        catch (Exception)
        {
            // Building the check-status response needs the full Functions host; only scheduling is verified here.
        }

        await client.Received(1).ScheduleNewOrchestrationInstanceAsync(
            nameof(ForceTableResyncOrchestrator),
            "account",
            Arg.Is<StartOrchestrationOptions?>(o => o != null && o.InstanceId == instanceId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Run_InstanceCompleted_AllowsNewRun()
    {
        var (request, _) = HttpTestHelper.CreateRequest();
        var client = Substitute.For<DurableTaskClient>("test");
        var instanceId = ForceTableResyncInstanceId.For("account");
        client.GetInstanceAsync(instanceId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(new OrchestrationMetadata(nameof(ForceTableResyncOrchestrator), instanceId)
            {
                RuntimeStatus = OrchestrationRuntimeStatus.Completed
            });
        client.ScheduleNewOrchestrationInstanceAsync(
                Arg.Any<TaskName>(), Arg.Any<object?>(), Arg.Any<StartOrchestrationOptions?>(), Arg.Any<CancellationToken>())
            .Returns(instanceId);

        try
        {
            await _starter.Run(request, client, "account");
        }
        catch (Exception)
        {
            // See comment in the previous test.
        }

        await client.Received(1).ScheduleNewOrchestrationInstanceAsync(
            Arg.Any<TaskName>(), Arg.Any<object?>(), Arg.Any<StartOrchestrationOptions?>(), Arg.Any<CancellationToken>());
    }
}
