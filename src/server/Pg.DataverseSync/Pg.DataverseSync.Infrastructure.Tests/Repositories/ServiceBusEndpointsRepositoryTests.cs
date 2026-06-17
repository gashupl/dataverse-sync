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
    public class ServiceBusEndpointsRepositoryTests : RepositoryTestsBase
    {
        [Fact]
        public void StepExists_WhenMessageFilterDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var serviceFactory = BuildServiceFactoryWithFilterAndStep(out var serviceEndpointId, includeStep: false, includeFilter: false);
            var repository = new ServiceBusEndpointsRepository(serviceFactory, tracingService);

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
            var repository = new ServiceBusEndpointsRepository(serviceFactory, tracingService);

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
            var repository = new ServiceBusEndpointsRepository(serviceFactory, tracingService);

            // Act
            var result = repository.StepExists(serviceEndpointId, "Create", "account");

            // Assert
            Assert.True(result);
        }

        private IOrganizationServiceFactory BuildServiceFactoryWithFilterAndStep(out Guid serviceEndpointId, bool includeStep, bool includeFilter = true)
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

            var entities = new List<Entity> { sdkMessage };

            if (includeFilter)
            {
                var filter = new SdkMessageFilter { Id = filterId };
                filter["primaryobjecttypecode"] = "account";
                filter["sdkmessageid"] = new EntityReference("sdkmessage", sdkMessageId);
                entities.Add(filter);
            }

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
    }
}
