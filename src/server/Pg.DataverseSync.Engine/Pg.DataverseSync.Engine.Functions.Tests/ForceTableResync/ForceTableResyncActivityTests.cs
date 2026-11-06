using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Core.Schema;
using Pg.DataverseSync.Engine.Functions.ForceTableResync;

namespace Pg.DataverseSync.Engine.Functions.Tests.ForceTableResync;

[ExcludeFromCodeCoverage]
public class ForceTableResyncActivityTests
{
    private readonly ISyncMetadataService _syncMetadataService = Substitute.For<ISyncMetadataService>();
    private readonly IDataLoadService _dataLoadService = Substitute.For<IDataLoadService>();
    private readonly ILogger<ForceTableResyncActivity> _logger = Substitute.For<ILogger<ForceTableResyncActivity>>();

    public ForceTableResyncActivityTests()
    {
        _syncMetadataService.PurgeTable(Arg.Any<string>())
            .Returns(callInfo => new PurgeTableResult(callInfo.ArgAt<string>(0), true));
    }

    private ForceTableResyncActivity CreateActivity()
    {
        return new ForceTableResyncActivity(_syncMetadataService, _dataLoadService, _logger);
    }

    private static SyncMetadataResult CreateResult(TableSyncResult tableResult)
    {
        var result = new SyncMetadataResult();
        result.TablesSyncResult.Add(tableResult);
        return result;
    }

    [Fact]
    public void Constructor_NullSyncMetadataService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ForceTableResyncActivity(null!, _dataLoadService, _logger));
    }

    [Fact]
    public void Constructor_NullDataLoadService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ForceTableResyncActivity(_syncMetadataService, null!, _logger));
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ForceTableResyncActivity(_syncMetadataService, _dataLoadService, null!));
    }

    [Fact]
    public async Task Run_CreateOperation_LoadsInitialDataOnce()
    {
        _syncMetadataService.Execute("account")
            .Returns(CreateResult(new TableSyncResult("account", true, TableSyncOperationCode.Create)));

        await CreateActivity().Run("account");

        _dataLoadService.Received(1).LoadInitialData("account");
    }

    [Fact]
    public async Task Run_NonCreateOperation_DoesNotLoadInitialData()
    {
        _syncMetadataService.Execute("account")
            .Returns(CreateResult(new TableSyncResult("account", true, TableSyncOperationCode.None)));

        await CreateActivity().Run("account");

        _dataLoadService.DidNotReceive().LoadInitialData(Arg.Any<string>());
    }

    [Fact]
    public async Task Run_SynchronizationFails_DoesNotLoadInitialData()
    {
        _syncMetadataService.Execute("account")
            .Returns(CreateResult(new TableSyncResult("account", false, TableSyncOperationCode.None, "failed")));

        await CreateActivity().Run("account");

        _dataLoadService.DidNotReceive().LoadInitialData(Arg.Any<string>());
    }

    [Fact]
    public async Task Run_NullTablesSyncResult_DoesNotLoadInitialData()
    {
        _syncMetadataService.Execute("account").Returns(new SyncMetadataResult { TablesSyncResult = null! });

        await CreateActivity().Run("account");

        _dataLoadService.DidNotReceive().LoadInitialData(Arg.Any<string>());
    }

    [Fact]
    public async Task Run_EmptyTablesSyncResult_DoesNotThrow()
    {
        _syncMetadataService.Execute("account").Returns(new SyncMetadataResult());

        await CreateActivity().Run("account");

        _dataLoadService.DidNotReceive().LoadInitialData(Arg.Any<string>());
    }

    [Fact]
    public async Task Run_AlwaysExecutesSyncForGivenTable()
    {
        _syncMetadataService.Execute("account")
            .Returns(CreateResult(new TableSyncResult("account", true, TableSyncOperationCode.None)));

        await CreateActivity().Run("account");

        _syncMetadataService.Received(1).Execute("account");
    }

    [Fact]
    public async Task Run_MultipleSynchronizedResults_DoesNotLoadInitialData()
    {
        var result = new SyncMetadataResult();
        result.TablesSyncResult.Add(new TableSyncResult("account", true, TableSyncOperationCode.Create));
        result.TablesSyncResult.Add(new TableSyncResult("contact", true, TableSyncOperationCode.Create));
        _syncMetadataService.Execute("account").Returns(result);

        await CreateActivity().Run("account");

        _dataLoadService.DidNotReceive().LoadInitialData(Arg.Any<string>());
    }

    [Fact]
    public async Task Run_NullResult_DoesNotLoadInitialData()
    {
        _syncMetadataService.Execute("account").Returns((SyncMetadataResult)null!);

        await CreateActivity().Run("account");

        _dataLoadService.DidNotReceive().LoadInitialData(Arg.Any<string>());
    }

    [Fact]
    public async Task Run_FailedResultWithoutMessage_DoesNotThrow()
    {
        _syncMetadataService.Execute("account")
            .Returns(CreateResult(new TableSyncResult("account", false, TableSyncOperationCode.None)));

        await CreateActivity().Run("account");

        _dataLoadService.DidNotReceive().LoadInitialData(Arg.Any<string>());
    }

    [Fact]
    public async Task Run_PurgeFails_DoesNotExecuteSyncOrLoadInitialData()
    {
        _syncMetadataService.PurgeTable("account").Returns(new PurgeTableResult("account", false, "purge failed"));

        await CreateActivity().Run("account");

        _syncMetadataService.Received(1).PurgeTable("account");
        _syncMetadataService.DidNotReceive().Execute(Arg.Any<string>());
        _dataLoadService.DidNotReceive().LoadInitialData(Arg.Any<string>());
    }

    [Fact]
    public async Task Run_NullPurgeResult_DoesNotExecuteSyncOrLoadInitialData()
    {
        _syncMetadataService.PurgeTable("account").Returns((PurgeTableResult)null!);

        await CreateActivity().Run("account");

        _syncMetadataService.Received(1).PurgeTable("account");
        _syncMetadataService.DidNotReceive().Execute(Arg.Any<string>());
        _dataLoadService.DidNotReceive().LoadInitialData(Arg.Any<string>());
    }
}
