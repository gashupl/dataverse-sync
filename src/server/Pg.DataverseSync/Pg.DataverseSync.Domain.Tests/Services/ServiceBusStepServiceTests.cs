using System;
using Microsoft.Xrm.Sdk;
using Moq;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Domain.Services;
using Pg.DataverseSync.Model;
using Xunit;

namespace Pg.DataverseSync.Domain.Tests.Services
{
    public class ServiceBusStepServiceTests
    {
        private readonly Mock<IServiceBusEndpointsRepository> _mockServiceBusEndpointsRepository;
        private readonly Mock<IEnvironmentVariablesRepository> _mockEnvVariablesRepository;
        private readonly Mock<ITracingService> _mockTracingService;
        private readonly ServiceBusStepService _service;
        private readonly Guid _serviceEndpointId = Guid.NewGuid();
        private readonly Guid _sdkMessageId = Guid.NewGuid();
        private readonly Guid _sdkMessageFilterId = Guid.NewGuid();

        public ServiceBusStepServiceTests()
        {
            _mockServiceBusEndpointsRepository = new Mock<IServiceBusEndpointsRepository>();
            _mockEnvVariablesRepository = new Mock<IEnvironmentVariablesRepository>();
            _mockTracingService = new Mock<ITracingService>();

            _mockEnvVariablesRepository.Setup(x => x.GetValue(It.IsAny<string>())).Returns(_serviceEndpointId.ToString());
            _mockServiceBusEndpointsRepository.Setup(x => x.GetSdkMessageId(It.IsAny<string>())).Returns(_sdkMessageId);
            _mockServiceBusEndpointsRepository.Setup(x => x.GetSdkMessageFilterId(It.IsAny<string>(), It.IsAny<string>())).Returns(_sdkMessageFilterId);

            _service = new ServiceBusStepService(_mockEnvVariablesRepository.Object, _mockServiceBusEndpointsRepository.Object, _mockTracingService.Object);
        }

        [Fact]
        public void CreateStepsForEntity_Success_ReturnsSuccessResult()
        {
            var result = _service.CreateStepsForEntity("account", new[] { "Create" });

            Assert.True(result.Success);
            Assert.Equal(string.Empty, result.ErrorMessage);
        }

        [Fact]
        public void CreateStepsForEntity_MultipleMessages_CallsRepositoryCreateStepForEach()
        {
            _service.CreateStepsForEntity("account", new[] { "Create", "Update", "Delete" });

            _mockServiceBusEndpointsRepository.Verify(
                x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>(), "account"),
                Times.Exactly(3));
        }

        [Fact]
        public void CreateStepsForEntity_RepositoryThrows_ReturnsFailureResult()
        {
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Connection failed"));

            var result = _service.CreateStepsForEntity("account", new[] { "Create" });

            Assert.False(result.Success);
            Assert.Equal("Connection failed", result.ErrorMessage);
        }

        [Fact]
        public void CreateStepsForEntity_RepositoryThrows_TracesError()
        {
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Connection failed"));

            _service.CreateStepsForEntity("account", new[] { "Create" });

            _mockTracingService.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("account") && s.Contains("Connection failed"))),
                Times.Once);
        }

        [Fact]
        public void CreateStepsForEntity_PassesEntityNameToRepository()
        {
            _service.CreateStepsForEntity("contact", new[] { "Update" });

            _mockServiceBusEndpointsRepository.Verify(
                x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), "Update", "contact"),
                Times.Once);
        }

        [Fact]
        public void CreateStepsForEntity_SetsStepNameWithEntityAndMessage()
        {
            SdkMessageProcessingStep capturedStep = null;
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<SdkMessageProcessingStep, string, string>((step, _, __) => capturedStep = step);

            _service.CreateStepsForEntity("account", new[] { "Create" });

            Assert.Equal("DataverseSync Endpoint: Create to account", capturedStep.Name);
        }

        [Fact]
        public void CreateStepsForEntity_SetsEventHandlerToServiceEndpoint()
        {
            SdkMessageProcessingStep capturedStep = null;
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<SdkMessageProcessingStep, string, string>((step, _, __) => capturedStep = step);

            _service.CreateStepsForEntity("account", new[] { "Create" });

            Assert.Equal("serviceendpoint", capturedStep.EventHandler.LogicalName);
            Assert.Equal(_serviceEndpointId, capturedStep.EventHandler.Id);
        }

        [Fact]
        public void CreateStepsForEntity_SetsAsynchronousMode()
        {
            SdkMessageProcessingStep capturedStep = null;
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<SdkMessageProcessingStep, string, string>((step, _, __) => capturedStep = step);

            _service.CreateStepsForEntity("account", new[] { "Create" });

            Assert.Equal(SdkMessageProcessingStep_Mode.Asynchronous, capturedStep.Mode);
        }

        [Fact]
        public void CreateStepsForEntity_SetsPostOperationStage()
        {
            SdkMessageProcessingStep capturedStep = null;
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<SdkMessageProcessingStep, string, string>((step, _, __) => capturedStep = step);

            _service.CreateStepsForEntity("account", new[] { "Delete" });

            Assert.Equal(SdkMessageProcessingStep_Stage.PostOperation, capturedStep.Stage);
        }

        [Fact]
        public void CreateStepsForEntity_SetsSdkMessageId()
        {
            SdkMessageProcessingStep capturedStep = null;
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<SdkMessageProcessingStep, string, string>((step, _, __) => capturedStep = step);

            _service.CreateStepsForEntity("account", new[] { "Create" });

            Assert.Equal(SdkMessage.EntityLogicalName, capturedStep.SdkMessageId.LogicalName);
        }

        [Fact]
        public void CreateStepsForEntity_WhenEnvironmentVariableDoesNotExist_ReturnsFailureResult()
        {
            var mockEnvRepo = new Mock<IEnvironmentVariablesRepository>();
            mockEnvRepo.Setup(x => x.GetValue(It.IsAny<string>())).Returns((string)null);
            var service = new ServiceBusStepService(mockEnvRepo.Object, _mockServiceBusEndpointsRepository.Object, _mockTracingService.Object);

            var result = service.CreateStepsForEntity("account", new[] { "Create" });

            Assert.False(result.Success);
            Assert.Equal("Missing or invalid ServiceEndpointId.", result.ErrorMessage);
        }

        [Fact]
        public void DeleteStepsForEntity_Success_ReturnsSuccessResult()
        {
            var result = _service.DeleteStepsForEntity("account", new[] { "Create" });

            Assert.True(result.Success);
            Assert.Equal(string.Empty, result.ErrorMessage);
        }

        [Fact]
        public void DeleteStepsForEntity_MultipleMessages_CallsRepositoryDeleteStepForEach()
        {
            _service.DeleteStepsForEntity("account", new[] { "Create", "Update", "Delete" });

            _mockServiceBusEndpointsRepository.Verify(
                x => x.DeleteStep(_serviceEndpointId, It.IsAny<string>(), "account"),
                Times.Exactly(3));
        }

        [Fact]
        public void DeleteStepsForEntity_Success_CallsRepositoryDeleteStep()
        {
            _service.DeleteStepsForEntity("account", new[] { "Create" });

            _mockServiceBusEndpointsRepository.Verify(
                x => x.DeleteStep(_serviceEndpointId, "Create", "account"),
                Times.Once);
        }

        [Fact]
        public void DeleteStepsForEntity_RepositoryThrows_ReturnsFailureResult()
        {
            _mockServiceBusEndpointsRepository
                .Setup(x => x.DeleteStep(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Delete failed"));

            var result = _service.DeleteStepsForEntity("account", new[] { "Create" });

            Assert.False(result.Success);
            Assert.Equal("Delete failed", result.ErrorMessage);
        }

        [Fact]
        public void DeleteStepsForEntity_RepositoryThrows_TracesError()
        {
            _mockServiceBusEndpointsRepository
                .Setup(x => x.DeleteStep(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Delete failed"));

            _service.DeleteStepsForEntity("account", new[] { "Create" });

            _mockTracingService.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("account") && s.Contains("Delete failed"))),
                Times.Once);
        }

        [Fact]
        public void DeleteStepsForEntity_WhenEnvironmentVariableDoesNotExist_ReturnsFailureResult()
        {
            var mockEnvRepo = new Mock<IEnvironmentVariablesRepository>();
            mockEnvRepo.Setup(x => x.GetValue(It.IsAny<string>())).Returns((string)null);
            var service = new ServiceBusStepService(mockEnvRepo.Object, _mockServiceBusEndpointsRepository.Object, _mockTracingService.Object);

            var result = service.DeleteStepsForEntity("account", new[] { "Create" });

            Assert.False(result.Success);
            Assert.Equal("Missing or invalid ServiceEndpointId.", result.ErrorMessage);
        }
    }
}
