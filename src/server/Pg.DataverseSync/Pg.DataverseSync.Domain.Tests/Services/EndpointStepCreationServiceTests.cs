using System;
using Microsoft.Xrm.Sdk;
using Moq;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Domain.Services;
using Pg.DataverseSync.Model;
using Xunit;

namespace Pg.DataverseSync.Domain.Tests.Services
{
    public class EndpointStepCreationServiceTests
    {
        private readonly Mock<IRepository> _mockRepository;
        private readonly Mock<ITracingService> _mockTracingService;
        private readonly EndpointStepCreationService _service;

        public EndpointStepCreationServiceTests()
        {
            _mockRepository = new Mock<IRepository>();
            _mockTracingService = new Mock<ITracingService>();
            _service = new EndpointStepCreationService(_mockRepository.Object, _mockTracingService.Object);
        }

        [Fact]
        public void CreateStepForEntity_Success_ReturnsSuccessResult()
        {
            var result = _service.CreateStepForEntity("account");

            Assert.True(result.Success);
            Assert.Equal(string.Empty, result.ErrorMessage);
        }

        [Fact]
        public void CreateStepForEntity_Success_CallsRepositoryCreateStep()
        {
            _service.CreateStepForEntity("account");

            _mockRepository.Verify(
                x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), "account"),
                Times.Once);
        }

        [Fact]
        public void CreateStepForEntity_RepositoryThrows_ReturnsFailureResult()
        {
            _mockRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>()))
                .Throws(new Exception("Connection failed"));

            var result = _service.CreateStepForEntity("account");

            Assert.False(result.Success);
            Assert.Equal("Connection failed", result.ErrorMessage);
        }

        [Fact]
        public void CreateStepForEntity_RepositoryThrows_TracesError()
        {
            _mockRepository
                .Setup(x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), It.IsAny<string>()))
                .Throws(new Exception("Connection failed"));

            _service.CreateStepForEntity("account");

            _mockTracingService.Verify(
                x => x.Trace(It.Is<string>(s => s.Contains("account") && s.Contains("Connection failed"))),
                Times.Once);
        }

        [Fact]
        public void CreateStepForEntity_PassesEntityNameToRepository()
        {
            var entityName = "contact";

            _service.CreateStepForEntity(entityName);

            _mockRepository.Verify(
                x => x.CreateStep(It.IsAny<SdkMessageProcessingStep>(), entityName),
                Times.Once);
        }
    }
}
