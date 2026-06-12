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
        public void CreateStepForEntity_Success_ReturnsSuccessResult()
        {
            var result = _service.CreateStepForEntity("account", "Create");

            Assert.True(result.Success);
            Assert.Equal(string.Empty, result.ErrorMessage);
        }

        [Fact]
        public void CreateStepForEntity_Success_CallsRepositoryCreateStep()
        {
            _service.CreateStepForEntity("account", "Create");

            _mockServiceBusEndpointsRepository.Verify(
                x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), "account"),
                Times.Once);
        }

        [Fact]
        public void CreateStepForEntity_RepositoryThrows_ReturnsFailureResult()
        {
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>()))
                .Throws(new Exception("Connection failed"));

            var result = _service.CreateStepForEntity("account", "Create");

            Assert.False(result.Success);
            Assert.Equal("Connection failed", result.ErrorMessage);
        }

        [Fact]
        public void CreateStepForEntity_RepositoryThrows_TracesError()
        {
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>()))
                .Throws(new Exception("Connection failed"));

            _service.CreateStepForEntity("account", "Create");

            _mockTracingService.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("account") && s.Contains("Connection failed"))),
                Times.Once);
        }

        [Fact]
        public void CreateStepForEntity_PassesEntityNameToRepository()
        {
            _service.CreateStepForEntity("contact", "Update");

            _mockServiceBusEndpointsRepository.Verify(
                x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), "contact"),
                Times.Once);
        }

        [Fact]
        public void CreateStepForEntity_SetsStepNameWithEntityAndMessage()
        {
            SdkMessageProcessingStep capturedStep = null;
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>()))
                .Callback<SdkMessageProcessingStep, string>((step, _) => capturedStep = step);

            _service.CreateStepForEntity("account", "Create");

            Assert.Equal("DataverseSync Endpoint: Create to account", capturedStep.Name);
        }

        [Fact]
        public void CreateStepForEntity_SetsEventHandlerToServiceEndpoint()
        {
            SdkMessageProcessingStep capturedStep = null;
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>()))
                .Callback<SdkMessageProcessingStep, string>((step, _) => capturedStep = step);

            _service.CreateStepForEntity("account", "Create");

            Assert.Equal("serviceendpoint", capturedStep.EventHandler.LogicalName);
            Assert.Equal(_serviceEndpointId, capturedStep.EventHandler.Id);
        }

        [Fact]
        public void CreateStepForEntity_SetsAsynchronousMode()
        {
            SdkMessageProcessingStep capturedStep = null;
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>()))
                .Callback<SdkMessageProcessingStep, string>((step, _) => capturedStep = step);

            _service.CreateStepForEntity("account", "Create");

            Assert.Equal(SdkMessageProcessingStep_Mode.Asynchronous, capturedStep.Mode);
        }

        [Fact]
        public void CreateStepForEntity_SetsPostOperationStage()
        {
            SdkMessageProcessingStep capturedStep = null;
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>()))
                .Callback<SdkMessageProcessingStep, string>((step, _) => capturedStep = step);

            _service.CreateStepForEntity("account", "Delete");

            Assert.Equal(SdkMessageProcessingStep_Stage.PostOperation, capturedStep.Stage);
        }

        [Fact]
        public void CreateStepForEntity_SetsSdkMessageId()
        {
            SdkMessageProcessingStep capturedStep = null;
            _mockServiceBusEndpointsRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>()))
                .Callback<SdkMessageProcessingStep, string>((step, _) => capturedStep = step);

            _service.CreateStepForEntity("account", "Create");

            Assert.Equal(SdkMessage.EntityLogicalName, capturedStep.SdkMessageId.LogicalName);
        }

        [Fact]
        public void CreateStepForEntity_WhenEnvironmentVariableDoesNotExist_ReturnsFailureResult()
        {
            var mockEnvRepo = new Mock<IEnvironmentVariablesRepository>();
            mockEnvRepo.Setup(x => x.GetValue(It.IsAny<string>())).Returns((string)null);
            var service = new ServiceBusStepService(mockEnvRepo.Object, _mockServiceBusEndpointsRepository.Object, _mockTracingService.Object);

            var result = service.CreateStepForEntity("account", "Create");

            Assert.False(result.Success);
            Assert.Equal("Missing or invalid ServiceEndpointId.", result.ErrorMessage);
        }

        [Fact]
        public void DeleteStepForEntity_Success_ReturnsSuccessResult()
        {
            var result = _service.DeleteStepForEntity("account", "Create");

            Assert.True(result.Success);
            Assert.Equal(string.Empty, result.ErrorMessage);
        }

        [Fact]
        public void DeleteStepForEntity_Success_CallsRepositoryDeleteStep()
        {
            _service.DeleteStepForEntity("account", "Create");

            _mockServiceBusEndpointsRepository.Verify(
                x => x.DeleteStep(_serviceEndpointId, "Create", "account"),
                Times.Once);
        }

        [Fact]
        public void DeleteStepForEntity_RepositoryThrows_ReturnsFailureResult()
        {
            _mockServiceBusEndpointsRepository
                .Setup(x => x.DeleteStep(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Delete failed"));

            var result = _service.DeleteStepForEntity("account", "Create");

            Assert.False(result.Success);
            Assert.Equal("Delete failed", result.ErrorMessage);
        }

        [Fact]
        public void DeleteStepForEntity_RepositoryThrows_TracesError()
        {
            _mockServiceBusEndpointsRepository
                .Setup(x => x.DeleteStep(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Delete failed"));

            _service.DeleteStepForEntity("account", "Create");

            _mockTracingService.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("account") && s.Contains("Delete failed"))),
                Times.Once);
        }

        [Fact]
        public void DeleteStepForEntity_WhenEnvironmentVariableDoesNotExist_ReturnsFailureResult()
        {
            var mockEnvRepo = new Mock<IEnvironmentVariablesRepository>();
            mockEnvRepo.Setup(x => x.GetValue(It.IsAny<string>())).Returns((string)null);
            var service = new ServiceBusStepService(mockEnvRepo.Object, _mockServiceBusEndpointsRepository.Object, _mockTracingService.Object);

            var result = service.DeleteStepForEntity("account", "Create");

            Assert.False(result.Success);
            Assert.Equal("Missing or invalid ServiceEndpointId.", result.ErrorMessage);
        }
    }
}
