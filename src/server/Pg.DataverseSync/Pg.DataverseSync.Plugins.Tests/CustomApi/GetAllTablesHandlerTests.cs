using Microsoft.Xrm.Sdk;
using Moq;
using Pg.DataverseSync.Domain.Dto;
using Pg.DataverseSync.Domain.Services;
using Pg.DataverseSync.Model;
using Pg.DataverseSync.Plugins.CustomApi;
using Pg.DataverseSync.Plugins.Tests.Core;
using System;
using System.Collections.Generic;
using Xunit;

namespace Pg.DataverseSync.Plugins.Tests.CustomApi
{
    public class GetAllTablesHandlerTests : PluginHandlerTestBase
    {
        [Fact]
        public void GetAllTablesHandler_CanExecute_ReturnsTrue()
        {
            var tablesService = new Mock<ITablesService>();
            var parseToJsonService = new Mock<IParseToJsonService>();
            var handler = new GetAllTablesHandler(tablesService.Object, parseToJsonService.Object);

            var result = handler.CanExecute();

            Assert.True(result);
        }

        [Fact]
        public void GetAllTablesHandler_Execute_NullLocalPluginContext_ThrowsInvalidOperationException()
        {
            var tablesService = new Mock<ITablesService>();
            var parseToJsonService = new Mock<IParseToJsonService>();
            var handler = new GetAllTablesHandler(tablesService.Object, parseToJsonService.Object);

            Assert.Throws<InvalidOperationException>(() => handler.Execute());
        }

        [Fact]
        public void GetAllTablesHandler_Execute_MapsOutputParameterWithParsedTables()
        {
            var expectedJson = "[{\"SchemaName\":\"pg_test1\"},{\"SchemaName\":\"pg_test2\"}]";
            var tables = new List<Table>
            {
                new Table { Name = "Test 1", SchemaName = "pg_test1" },
                new Table { Name = "Test 2", SchemaName = "pg_test2" }
            };

            var tablesService = new Mock<ITablesService>();
            tablesService.Setup(s => s.GetAllTables()).Returns(tables);

            var parseToJsonService = new Mock<IParseToJsonService>();
            parseToJsonService.Setup(s => s.Parse(tables)).Returns(expectedJson);

            var handler = new GetAllTablesHandler(tablesService.Object, parseToJsonService.Object);
            var localPluginContext = CreateLocalPluginContext();
            handler.Init(localPluginContext);

            handler.Execute();

            Assert.Equal(expectedJson, localPluginContext.PluginExecutionContext.OutputParameters[pg_gettablesResponse.Fields.alltables]);
            tablesService.Verify(s => s.GetAllTables(), Times.Once);
            parseToJsonService.Verify(s => s.Parse(tables), Times.Once);
        }
    }
}
