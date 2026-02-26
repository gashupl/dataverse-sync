using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pg.DataverseSync.Engine.Core.Exceptions;
using System.ServiceModel;

namespace Pg.DataverseSync.Engine.Source.Tests
{
    public class MetadataReaderTests
    {
        [Fact]
        public void GetTables_SuccessfullExecute_ReturnListOfTables()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataReader>>();

            var entityMetadata1 = new EntityMetadata
            {
                LogicalName = "account",
                IsActivity = false
            };
            entityMetadata1.DisplayName = new Label(new LocalizedLabel("Account", 1033), Array.Empty<LocalizedLabel>());

            var entityMetadata2 = new EntityMetadata
            {
                LogicalName = "contact",
                IsActivity = false
            };
            entityMetadata2.DisplayName = new Label(new LocalizedLabel("Contact", 1033), Array.Empty<LocalizedLabel>());

            var response = new RetrieveAllEntitiesResponse
            {
                Results = new ParameterCollection
                {
                    ["EntityMetadata"] = new EntityMetadata[] { entityMetadata1, entityMetadata2 }
                }
            };

            mockService.Execute(Arg.Any<RetrieveAllEntitiesRequest>()).Returns(response);

            var metadataReader = new MetadataReader(mockService, mockLogger);

            // Act
            var result = metadataReader.GetTables();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("account", result[0].Name);
            Assert.Equal("Account", result[0].DisplayName);
            Assert.False(result[0].IsActivity);
            Assert.Equal("contact", result[1].Name);
            Assert.Equal("Contact", result[1].DisplayName);
            Assert.False(result[1].IsActivity);

            mockService.Received(1).Execute(Arg.Any<RetrieveAllEntitiesRequest>());
        }

        [Fact]
        public void GetTables_FailedExecute_ThrowsFaultException()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataReader>>();
            var faultException = new FaultException<OrganizationServiceFault>(new OrganizationServiceFault
            {
                ErrorCode = -2147220969,
                Message = "An error occurred while processing the request."
            });
            mockService.Execute(Arg.Any<RetrieveAllEntitiesRequest>()).Throws(faultException);
            var metadataReader = new MetadataReader(mockService, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ReadMetadataException>(() => metadataReader.GetTables());
            Assert.Contains("Dataverse service fault while retrieving tables", exception.Message);
            Assert.IsType<FaultException<OrganizationServiceFault>>(exception.InnerException);
        }

        [Fact]
        public void GetTables_FailedExecute_ThrowsTimeoutException()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataReader>>();
            mockService.Execute(Arg.Any<RetrieveAllEntitiesRequest>()).Throws(new TimeoutException("The operation has timed out."));
            var metadataReader = new MetadataReader(mockService, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ReadMetadataException>(() => metadataReader.GetTables());
            Assert.Contains("Timeout while retrieving tables from Dataverse", exception.Message);
            Assert.IsType<TimeoutException>(exception.InnerException);
        }

        [Fact]
        public void GetTables_FailedExecute_ThrowsGenericException()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataReader>>();
            mockService.Execute(Arg.Any<RetrieveAllEntitiesRequest>()).Throws(new Exception("Unexpected error"));
            var metadataReader = new MetadataReader(mockService, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ReadMetadataException>(() => metadataReader.GetTables());
            Assert.Contains("An unexpected error occurred while retrieving tables from Dataverse", exception.Message);
            Assert.IsType<Exception>(exception.InnerException);
        }

        [Fact]
        public void GetColumns_SuccessfullExecute_ReturnListOfColumns()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataReader>>();

            var attribute1 = new StringAttributeMetadata
            {
                LogicalName = "name"
            };

            var attribute2 = new UniqueIdentifierAttributeMetadata
            {
                LogicalName = "accountid"
            };

            var entityMetadata = new EntityMetadata
            {
                LogicalName = "account"
            };

            typeof(EntityMetadata)
                .GetProperty("Attributes")!
                .SetValue(entityMetadata, new AttributeMetadata[] { attribute1, attribute2 });

            var response = new RetrieveEntityResponse
            {
                Results = new ParameterCollection
                {
                    ["EntityMetadata"] = entityMetadata
                }
            };

            mockService.Execute(Arg.Any<RetrieveEntityRequest>()).Returns(response);

            var metadataReader = new MetadataReader(mockService, mockLogger);

            // Act
            var result = metadataReader.GetColumns("account");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("name", result[0].Name);
            Assert.Equal("StringType", result[0].DataType);
            Assert.False(result[0].IsPrimaryKey);
            Assert.True(result[0].IsNullable);
            Assert.Equal("accountid", result[1].Name);
            Assert.Equal("UniqueidentifierType", result[1].DataType);
            Assert.False(result[1].IsPrimaryKey);
            Assert.True(result[1].IsNullable);

            mockService.Received(1).Execute(Arg.Any<RetrieveEntityRequest>());
        }

        [Fact]
        public void GetColumns_FailedExecute_ThrowsFaultException()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataReader>>();
            var faultException = new FaultException<OrganizationServiceFault>(new OrganizationServiceFault
            {
                ErrorCode = -2147220969,
                Message = "An error occurred while processing the request."
            });
            mockService.Execute(Arg.Any<RetrieveEntityRequest>()).Throws(faultException);
            var metadataReader = new MetadataReader(mockService, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ReadMetadataException>(() => metadataReader.GetColumns("account"));
            Assert.IsType<FaultException<OrganizationServiceFault>>(exception.InnerException);
        }

        [Fact]
        public void GetColumns_FailedExecute_ThrowsTimeoutException()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataReader>>();
            mockService.Execute(Arg.Any<RetrieveEntityRequest>()).Throws(new TimeoutException("The operation has timed out."));
            var metadataReader = new MetadataReader(mockService, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ReadMetadataException>(() => metadataReader.GetColumns("account"));
            Assert.IsType<TimeoutException>(exception.InnerException);
        }

        [Fact]
        public void GetColumns_FailedExecute_ThrowsGenericException()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataReader>>();
            mockService.Execute(Arg.Any<RetrieveEntityRequest>()).Throws(new Exception("Unexpected error"));
            var metadataReader = new MetadataReader(mockService, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ReadMetadataException>(() => metadataReader.GetColumns("account"));
            Assert.IsType<Exception>(exception.InnerException);
        }
    }
}

