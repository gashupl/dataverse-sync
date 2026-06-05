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
    public class EnvironmentVariablesRepositoryTests
    {
        [Fact]
        public void GetValue_WhenEnvironmentVariableExists_ReturnsValue()
        {
            // Arrange
            var serviceFactory = BuildServiceFactoryWithEnvironmentVariable("pg_TestVariable", "TestValue");
            var repository = new EnvironmentVariablesRepository(serviceFactory);

            // Act
            var result = repository.GetValue("pg_TestVariable");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestValue", result);
        }

        [Fact]
        public void GetValue_WhenEnvironmentVariableDoesNotExist_ReturnsNull()
        {
            // Arrange
            var serviceFactory = BuildServiceFactoryWithEnvironmentVariable("pg_TestVariable", "TestValue");
            var repository = new EnvironmentVariablesRepository(serviceFactory);

            // Act
            var result = repository.GetValue("pg_NonExistentVariable");

            // Assert
            Assert.Null(result);
        }

        private IOrganizationServiceFactory BuildServiceFactoryWithEnvironmentVariable(string schemaName, string value)
        {
            IXrmFakedContext context = MiddlewareBuilder
                .New()
                .AddCrud()
                .AddFakeMessageExecutors(Assembly.GetAssembly(typeof(AddListMembersListRequestExecutor)))
                .UseCrud()
                .UseMessages()
                .SetLicense(FakeXrmEasyLicense.NonCommercial)
                .Build();

            context.EnableProxyTypes(Assembly.GetAssembly(typeof(EnvironmentVariableDefinition)));

            var definitionId = Guid.NewGuid();

            var definition = new EnvironmentVariableDefinition
            {
                Id = definitionId,
                SchemaName = schemaName
            };

            var variableValue = new EnvironmentVariableValue
            {
                Id = Guid.NewGuid(),
                EnvironmentVariableDefinitionId = new EntityReference(EnvironmentVariableDefinition.EntityLogicalName, definitionId),
                Value = value
            };

            context.Initialize(new List<Entity> { definition, variableValue });
            return new FakeOrganizationServiceFactory(context.GetOrganizationService());
        }
    }
}
