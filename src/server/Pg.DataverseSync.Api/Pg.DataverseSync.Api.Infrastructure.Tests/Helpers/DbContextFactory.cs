using Microsoft.EntityFrameworkCore;
using Pg.DataverseSync.Api.Infrastructure.Data;

namespace Pg.DataverseSync.Api.Infrastructure.Tests.Helpers;

public static class DbContextFactory
{
    public static AppDbContext CreateInMemoryDbContext(string databaseName = "TestDb")
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new AppDbContext(options);
    }
}
