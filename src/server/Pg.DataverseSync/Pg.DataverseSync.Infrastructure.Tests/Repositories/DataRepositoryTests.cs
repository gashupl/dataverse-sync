using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.FakeMessageExecutors;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
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
        public void GetActiveSynchronizedTables_ReturnExpectedResults()
        {
            // Arrange
            var repository = new DataRepository(_serviceFactory);   
            // Act
            var result = repository.GetActiveSynchronizedTables();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("pg_sample1", result[0].pg_name);
        }

        [Fact]
        public void GetTablesFromMetadata_ReturnsExpectedTables()
        {

            var repository = new DataRepository(_serviceFactory);
            var tables = repository.GetStandardTablesFromMetadata();

            Assert.NotNull(tables);
            Assert.Single(tables);
            Assert.Contains(tables, t => t.SchemaName == "pg_standardtable");
        }


        [Fact]
        public void StepExists_WhenMessageFilterDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var repository = new DataRepository(_serviceFactory);
            var serviceEndpointId = Guid.NewGuid();

            // Act
            var result = repository.StepExists(serviceEndpointId, "Create", "account");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void StepExists_WhenFilterExistsButNoStep_ReturnsFalse()
        {
            // Arrange
            var serviceFactory = BuildServiceFactoryWithFilterAndStep(out var serviceEndpointId, includeStep: false);
            var repository = new DataRepository(serviceFactory);

            // Act
            var result = repository.StepExists(serviceEndpointId, "Create", "account");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void StepExists_WhenFilterAndStepExist_ReturnsTrue()
        {
            // Arrange
            var serviceFactory = BuildServiceFactoryWithFilterAndStep(out var serviceEndpointId, includeStep: true);
            var repository = new DataRepository(serviceFactory);

            // Act
            var result = repository.StepExists(serviceEndpointId, "Create", "account");

            // Assert
            Assert.True(result);
        }

        private IOrganizationServiceFactory BuildServiceFactoryWithFilterAndStep(out Guid serviceEndpointId, bool includeStep)
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

            var sdkMessageId = Guid.NewGuid();
            var filterId = Guid.NewGuid();
            serviceEndpointId = Guid.NewGuid();

            var sdkMessage = new SdkMessage { Id = sdkMessageId, Name = "Create" };

            var filter = new SdkMessageFilter { Id = filterId };
            filter["primaryobjecttypecode"] = "account";
            filter["sdkmessageid"] = new EntityReference("sdkmessage", sdkMessageId);

            var entities = new List<Entity> { sdkMessage, filter };

            if (includeStep)
            {
                var step = new SdkMessageProcessingStep
                {
                    Id = Guid.NewGuid(),
                    EventHandler = new EntityReference("serviceendpoint", serviceEndpointId),
                    SdkMessageFilterId = new EntityReference(SdkMessageFilter.EntityLogicalName, filterId)
                };
                entities.Add(step);
            }

            context.Initialize(entities);
            return new FakeOrganizationServiceFactory(context.GetOrganizationService());
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

