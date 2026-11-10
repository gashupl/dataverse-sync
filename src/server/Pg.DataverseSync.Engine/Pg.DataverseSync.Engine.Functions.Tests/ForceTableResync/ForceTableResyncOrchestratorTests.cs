using System.Diagnostics.CodeAnalysis;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pg.DataverseSync.Engine.Functions.ForceTableResync;

namespace Pg.DataverseSync.Engine.Functions.Tests.ForceTableResync;

[ExcludeFromCodeCoverage]
public class ForceTableResyncOrchestratorTests
{
    [Fact]
    public async Task Run_CallsActivityWithTableName()
    {
        var context = Substitute.For<TaskOrchestrationContext>();
        var logger = Substitute.For<ILogger<ForceTableResyncOrchestrator>>();
        context.GetInput<string>().Returns("account");
        var orchestrator = new ForceTableResyncOrchestrator(logger);

        await orchestrator.Run(context);

        await context.Received(1).CallActivityAsync(
            nameof(ForceTableResyncActivity),
            "account",
            Arg.Any<TaskOptions?>());
    }
}
