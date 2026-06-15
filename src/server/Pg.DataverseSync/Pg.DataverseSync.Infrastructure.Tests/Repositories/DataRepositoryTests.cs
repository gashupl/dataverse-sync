using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.FakeMessageExecutors;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Moq;
using Pg.DataverseSync.Infrastructure.Core;
using Pg.DataverseSync.Infrastructure.Repositories;
using Pg.DataverseSync.Infrastructure.Tests.Core;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Pg.DataverseSync.Infrastructure.Tests.Repositories
{ 
    public class DataRepositoryTests
    {
        private readonly IOrganizationServiceFactory _serviceFactory;

        public DataRepositoryTests()
        {
            IXrmFakedContext context = MiddlewareBuilder
                .New()
                .AddCrud()
                .AddFakeMessageExecutors(Assembly.GetAssembly(typeof(AddListMembersListRequestExecutor)))
                .AddExecutionMock<RetrieveAllEntitiesRequest>(req =>
                 {
                     var response = new RetrieveAllEntitiesResponse
                     {
                         Results = new ParameterCollection
                         {
                             ["EntityMetadata"] = new[]
                             {
                        new Microsoft.Xrm.Sdk.Metadata.EntityMetadata
                        {
                            LogicalName = "pg_standardtable",
                            DisplayName = new Label("Standard Table", 1033),
                            TableType = TableTypes.Standard
                        },
                        new Microsoft.Xrm.Sdk.Metadata.EntityMetadata
                        {
                            LogicalName = "pg_sampleelastictable",
                            DisplayName = new Label("Elastic Table", 1033),
                            TableType = TableTypes.Elastic
                        }
                    }
                         }
                     };
                     return response;
                 })

                .UseCrud()
                .UseMessages()
                .SetLicense(FakeXrmEasyLicense.NonCommercial)
                .Build();

            _serviceFactory = InitDataServiceFactory(context);
        }

        [Fact]
        public void GetTablesFromMetadata_ReturnsExpectedTables()
        {

            var repository = new DataRepository(_serviceFactory, new Mock<ITracingService>().Object);
            var tables = repository.GetStandardTablesFromMetadata();

            Assert.NotNull(tables);
            Assert.Single(tables);
            Assert.Contains(tables, t => t.SchemaName == "pg_standardtable");
        }


        private IOrganizationServiceFactory InitDataServiceFactory(IXrmFakedContext context)
        {
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
            return new FakeOrganizationServiceFactory(context.GetOrganizationService());

        }

    }
}

