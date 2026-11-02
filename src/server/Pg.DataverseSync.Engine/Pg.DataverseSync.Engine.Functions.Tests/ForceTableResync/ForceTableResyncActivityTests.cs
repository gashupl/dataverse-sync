using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pg.DataverseSync.Engine.Functions.ForceTableResync;

namespace Pg.DataverseSync.Engine.Functions.Tests.ForceTableResync;

[ExcludeFromCodeCoverage]
public class ForceTableResyncActivityTests
{
    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ForceTableResyncActivity(null!));
    }

    [Fact]
    public async Task Run_CompletesSuccessfully()
    {
        var logger = Substitute.For<ILogger<ForceTableResyncActivity>>();
        var activity = new ForceTableResyncActivity(logger);

        await activity.Run("account");
    }
}
