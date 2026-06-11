using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.FakeMessageExecutors;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Infrastructure.Repositories;
using Pg.DataverseSync.Infrastructure.Tests.Core;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Pg.DataverseSync.Infrastructure.Tests.Repositories
{
    public class SyncTablesRepositoryTests
    {
        private readonly IOrganizationServiceFactory _serviceFactory;

        public SyncTablesRepositoryTests()
        {
            IXrmFakedContext context = MiddlewareBuilder
                .New()
                .AddCrud()
                .AddFakeMessageExecutors(Assembly.GetAssembly(typeof(AddListMembersListRequestExecutor)))
                .UseCrud()
                .UseMessages()
                .SetLicense(FakeXrmEasyLicense.NonCommercial)
                .Build();

            context.EnableProxyTypes(Assembly.GetAssembly(typeof(pg_synctable)));

            var record1 = new pg_synctable
            {
                Id = Guid.NewGuid(),
                pg_name = "pg_sample1",
                StateCode = pg_synctable_statecode.Active
            };

            var record2 = new pg_synctable
            {
                Id = Guid.NewGuid(),
                pg_name = "pg_sample2",
                StateCode = pg_synctable_statecode.Inactive
            };

            context.Initialize(new List<Entity>() { record1, record2 });
            _serviceFactory = new FakeOrganizationServiceFactory(context.GetOrganizationService());
        }

        [Fact]
        public void GetActiveSynchronizedTables_ReturnExpectedResults()
        {
            // Arrange
            var repository = new SyncTablesRepository(_serviceFactory);

            // Act
            var result = repository.GetActiveSynchronizedTables();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("pg_sample1", result[0].pg_name);
        }
    }
}
