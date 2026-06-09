using System;
using Microsoft.Xrm.Sdk;
using Moq;
using Pg.DataverseSync.Domain.Services;
using Pg.DataverseSync.Model;
using Pg.DataverseSync.Plugins.SyncTables;
using Xunit;

namespace Pg.DataverseSync.Plugins.Tests.SyncTables
{
    public class SyncTableServiceBusEndpointStepHandlerTests
    {
        private readonly Mock<IEndpointStepCreationService> _mockService;
        private readonly Mock<ILocalPluginContext> _mockLocalPluginContext;
        private readonly Mock<IPluginExecutionContext4> _mockExecutionContext;
        private readonly Mock<ITracingService> _mockTracingService;
        private readonly SyncTableServiceBusEndpointStepHandler _handler;

        public SyncTableServiceBusEndpointStepHandlerTests()
        {
            _mockService = new Mock<IEndpointStepCreationService>();
            _mockLocalPluginContext = new Mock<ILocalPluginContext>();
            _mockExecutionContext = new Mock<IPluginExecutionContext4>();
            _mockTracingService = new Mock<ITracingService>();

            _mockLocalPluginContext.Setup(x => x.PluginExecutionContext).Returns(_mockExecutionContext.Object);
            _mockLocalPluginContext.Setup(x => x.TracingService).Returns(_mockTracingService.Object);

            _handler = new SyncTableServiceBusEndpointStepHandler(_mockService.Object);
            _handler.Init(_mockLocalPluginContext.Object);
        }

        [Fact]
        public void CanExecute_CreateMessage_PostOperation_SyncTableEntity_ReturnsTrue()
        {
            _mockExecutionContext.Setup(x => x.MessageName).Returns("Create");
            _mockExecutionContext.Setup(x => x.Stage).Returns(40);
            _mockExecutionContext.Setup(x => x.PrimaryEntityName).Returns(pg_synctable.EntityLogicalName);

            var result = _handler.CanExecute();

            Assert.True(result);
        }

        [Fact]
        public void CanExecute_UpdateMessage_PostOperation_SyncTableEntity_ReturnsTrue()
        {
            _mockExecutionContext.Setup(x => x.MessageName).Returns("Update");
            _mockExecutionContext.Setup(x => x.Stage).Returns(40);
            _mockExecutionContext.Setup(x => x.PrimaryEntityName).Returns(pg_synctable.EntityLogicalName);

            var result = _handler.CanExecute();

            Assert.True(result);
        }

        [Fact]
        public void CanExecute_DeleteMessage_ReturnsFalse()
        {
            _mockExecutionContext.Setup(x => x.MessageName).Returns("Delete");
            _mockExecutionContext.Setup(x => x.Stage).Returns(40);
            _mockExecutionContext.Setup(x => x.PrimaryEntityName).Returns(pg_synctable.EntityLogicalName);

            var result = _handler.CanExecute();

            Assert.False(result);
        }

        [Fact]
        public void CanExecute_PreOperation_ReturnsFalse()
        {
            _mockExecutionContext.Setup(x => x.MessageName).Returns("Create");
            _mockExecutionContext.Setup(x => x.Stage).Returns(20);
            _mockExecutionContext.Setup(x => x.PrimaryEntityName).Returns(pg_synctable.EntityLogicalName);

            var result = _handler.CanExecute();

            Assert.False(result);
        }

        [Fact]
        public void CanExecute_DifferentEntity_ReturnsFalse()
        {
            _mockExecutionContext.Setup(x => x.MessageName).Returns("Create");
            _mockExecutionContext.Setup(x => x.Stage).Returns(40);
            _mockExecutionContext.Setup(x => x.PrimaryEntityName).Returns("account");

            var result = _handler.CanExecute();

            Assert.False(result);
        }

        [Fact]
        public void Execute_Create_Success_CallsCreateStepForEntity()
        {
            var target = new Entity(pg_synctable.EntityLogicalName, Guid.NewGuid());
            target[pg_synctable.Fields.pg_name] = "account";

            var inputParameters = new ParameterCollection { { "Target", target } };
            _mockExecutionContext.Setup(x => x.MessageName).Returns("Create");
            _mockExecutionContext.Setup(x => x.InputParameters).Returns(inputParameters);

            _mockService.Setup(x => x.CreateStepForEntity("account", "Create"))
                .Returns(new EndpointStepCreationResult { Success = true });

            _handler.Execute();

            _mockService.Verify(x => x.CreateStepForEntity("account", "Create"), Times.Once);
        }

        [Fact]
        public void Execute_Create_Failure_ThrowsInvalidPluginExecutionException()
        {
            var target = new Entity(pg_synctable.EntityLogicalName, Guid.NewGuid());
            target[pg_synctable.Fields.pg_name] = "account";

            var inputParameters = new ParameterCollection { { "Target", target } };
            _mockExecutionContext.Setup(x => x.MessageName).Returns("Create");
            _mockExecutionContext.Setup(x => x.InputParameters).Returns(inputParameters);

            _mockService.Setup(x => x.CreateStepForEntity("account", "Create"))
                .Returns(new EndpointStepCreationResult { Success = false, ErrorMessage = "Step already exists" });

            Assert.Throws<InvalidPluginExecutionException>(() => _handler.Execute());
        }

        [Fact]
        public void Execute_Create_ServiceThrowsException_ThrowsInvalidPluginExecutionException()
        {
            var target = new Entity(pg_synctable.EntityLogicalName, Guid.NewGuid());
            target[pg_synctable.Fields.pg_name] = "account";

            var inputParameters = new ParameterCollection { { "Target", target } };
            _mockExecutionContext.Setup(x => x.MessageName).Returns("Create");
            _mockExecutionContext.Setup(x => x.InputParameters).Returns(inputParameters);

            _mockService.Setup(x => x.CreateStepForEntity("account", "Create"))
                .Throws(new Exception("Unexpected error"));

            Assert.Throws<InvalidPluginExecutionException>(() => _handler.Execute());
        }

        [Fact]
        public void Execute_Update_NotReactivation_DoesNotCallService()
        {
            var target = new Entity(pg_synctable.EntityLogicalName, Guid.NewGuid());
            target[pg_synctable.Fields.pg_name] = "account";

            var inputParameters = new ParameterCollection { { "Target", target } };
            _mockExecutionContext.Setup(x => x.MessageName).Returns("Update");
            _mockExecutionContext.Setup(x => x.InputParameters).Returns(inputParameters);
            _mockExecutionContext.Setup(x => x.PreEntityImages).Returns(new EntityImageCollection());

            _handler.Execute();

            _mockService.Verify(x => x.CreateStepForEntity(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Execute_Update_Reactivation_CallsCreateStepForEntity()
        {
            var target = new Entity(pg_synctable.EntityLogicalName, Guid.NewGuid());
            target["statecode"] = new OptionSetValue(0); // Active

            var preImage = new Entity(pg_synctable.EntityLogicalName);
            preImage["statecode"] = new OptionSetValue(1); // Inactive
            preImage[pg_synctable.Fields.pg_name] = "contact";

            var preImages = new EntityImageCollection { { "PreImage", preImage } };
            var inputParameters = new ParameterCollection { { "Target", target } };

            _mockExecutionContext.Setup(x => x.MessageName).Returns("Update");
            _mockExecutionContext.Setup(x => x.InputParameters).Returns(inputParameters);
            _mockExecutionContext.Setup(x => x.PreEntityImages).Returns(preImages);

            _mockService.Setup(x => x.CreateStepForEntity("contact", "Create"))
                .Returns(new EndpointStepCreationResult { Success = true });

            _handler.Execute();

            _mockService.Verify(x => x.CreateStepForEntity("contact", "Create"), Times.Once);
        }

        [Fact]
        public void Execute_Update_Reactivation_WithNameOnTarget_UsesTargetName()
        {
            var target = new Entity(pg_synctable.EntityLogicalName, Guid.NewGuid());
            target["statecode"] = new OptionSetValue(0); // Active
            target[pg_synctable.Fields.pg_name] = "account";

            var preImage = new Entity(pg_synctable.EntityLogicalName);
            preImage["statecode"] = new OptionSetValue(1); // Inactive
            preImage[pg_synctable.Fields.pg_name] = "contact";

            var preImages = new EntityImageCollection { { "PreImage", preImage } };
            var inputParameters = new ParameterCollection { { "Target", target } };

            _mockExecutionContext.Setup(x => x.MessageName).Returns("Update");
            _mockExecutionContext.Setup(x => x.InputParameters).Returns(inputParameters);
            _mockExecutionContext.Setup(x => x.PreEntityImages).Returns(preImages);

            _mockService.Setup(x => x.CreateStepForEntity("account", "Create"))
                .Returns(new EndpointStepCreationResult { Success = true });

            _handler.Execute();

            _mockService.Verify(x => x.CreateStepForEntity("account", "Create"), Times.Once);
        }
    }
}
