using System.Diagnostics.CodeAnalysis;

namespace Pg.DataverseSync.Engine.Functions.Tests.ForceTableResync;

[ExcludeFromCodeCoverage]
public class ForceTableResyncInstanceIdTests
{
    [Fact]
    public void For_ReturnsPrefixedLowerCaseId()
    {
        var result = Functions.ForceTableResync.ForceTableResyncInstanceId.For("Account");

        Assert.Equal("force-table-resync-account", result);
    }

    [Fact]
    public void For_TrimsWhitespace()
    {
        var result = Functions.ForceTableResync.ForceTableResyncInstanceId.For("  Contact ");

        Assert.Equal("force-table-resync-contact", result);
    }

    [Fact]
    public void For_DifferentTables_ReturnDifferentIds()
    {
        Assert.NotEqual(
            Functions.ForceTableResync.ForceTableResyncInstanceId.For("account"),
            Functions.ForceTableResync.ForceTableResyncInstanceId.For("contact"));
    }
}
