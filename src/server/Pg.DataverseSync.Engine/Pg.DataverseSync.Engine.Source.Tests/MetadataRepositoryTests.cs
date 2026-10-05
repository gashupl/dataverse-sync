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
    public class MetadataRepositoryTests
    {
        [Fact]
        public void GetTables_SuccessfullExecute_ReturnListOfTables()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataRepository>>();

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

            var metadataRepo = new MetadataRepository(mockService, mockLogger);

            // Act
            var result = metadataRepo.GetTables();

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
            var mockLogger = Substitute.For<ILogger<MetadataRepository>>();
            var faultException = new FaultException<OrganizationServiceFault>(new OrganizationServiceFault
            {
                ErrorCode = -2147220969,
                Message = "An error occurred while processing the request."
            });
            mockService.Execute(Arg.Any<RetrieveAllEntitiesRequest>()).Throws(faultException);
            var metadataRepo = new MetadataRepository(mockService, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ReadMetadataException>(() => metadataRepo.GetTables());
            Assert.Contains("Dataverse service fault while retrieving tables", exception.Message);
            Assert.IsType<FaultException<OrganizationServiceFault>>(exception.InnerException);
        }

        [Fact]
        public void GetTables_FailedExecute_ThrowsTimeoutException()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataRepository>>();
            mockService.Execute(Arg.Any<RetrieveAllEntitiesRequest>()).Throws(new TimeoutException("The operation has timed out."));
            var metadataRepo = new MetadataRepository(mockService, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ReadMetadataException>(() => metadataRepo.GetTables());
            Assert.Contains("Timeout while retrieving tables from Dataverse", exception.Message);
            Assert.IsType<TimeoutException>(exception.InnerException);
        }

        [Fact]
        public void GetTables_FailedExecute_ThrowsGenericException()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataRepository>>();
            mockService.Execute(Arg.Any<RetrieveAllEntitiesRequest>()).Throws(new Exception("Unexpected error"));
            var metadataRepo = new MetadataRepository(mockService, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ReadMetadataException>(() => metadataRepo.GetTables());
            Assert.Contains("An unexpected error occurred while retrieving tables from Dataverse", exception.Message);
            Assert.IsType<Exception>(exception.InnerException);
        }

        [Fact]
        public void GetColumns_SuccessfullExecute_ReturnListOfColumns()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataRepository>>();

            var attribute1 = new StringAttributeMetadata
            {
                LogicalName = "name"
            };
            typeof(AttributeMetadata)
                .GetProperty("IsValidForRead")!
                .SetValue(attribute1, true);

            var attribute2 = new UniqueIdentifierAttributeMetadata
            {
                LogicalName = "accountid"
            };
            typeof(AttributeMetadata)
                .GetProperty("IsValidForRead")!
                .SetValue(attribute2, true);
            typeof(AttributeMetadata)
                .GetProperty("IsPrimaryId")!
                .SetValue(attribute2, true);

            var entityMetadata = new EntityMetadata
            {
                LogicalName = "account",
                IsActivity = false
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

            var metadataRepo = new MetadataRepository(mockService, mockLogger);

            // Act
            var result = metadataRepo.GetColumns("account");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("name", result[0].Name);
            Assert.Equal("StringType", result[0].DataType);
            Assert.False(result[0].IsPrimaryKey);
            Assert.True(result[0].IsNullable);
            Assert.Equal("accountid", result[1].Name);
            Assert.Equal("UniqueidentifierType", result[1].DataType);
            Assert.True(result[1].IsPrimaryKey);
            Assert.True(result[1].IsNullable);

            mockService.Received(1).Execute(Arg.Any<RetrieveEntityRequest>());
        }

        [Fact]
        public void GetColumns_PrimaryKeyDetectionForNonActivityTable_LowercaseTableNameComparison()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataRepository>>();

            var primaryKeyAttribute = new UniqueIdentifierAttributeMetadata
            {
                LogicalName = "contactid"
            };
            typeof(AttributeMetadata)
                .GetProperty("IsValidForRead")!
                .SetValue(primaryKeyAttribute, true);
            typeof(AttributeMetadata)
                .GetProperty("IsPrimaryId")!
                .SetValue(primaryKeyAttribute, true);

            var entityMetadata = new EntityMetadata
            {
                LogicalName = "contact",
                IsActivity = false
            };

            typeof(EntityMetadata)
                .GetProperty("Attributes")!
                .SetValue(entityMetadata, new AttributeMetadata[] { primaryKeyAttribute });

            var response = new RetrieveEntityResponse
            {
                Results = new ParameterCollection
                {
                    ["EntityMetadata"] = entityMetadata
                }
            };

            mockService.Execute(Arg.Any<RetrieveEntityRequest>()).Returns(response);

            var metadataRepo = new MetadataRepository(mockService, mockLogger);

            // Act
            var result = metadataRepo.GetColumns("contact");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("contactid", result[0].Name);
            Assert.True(result[0].IsPrimaryKey);
        }

        [Fact]
        public void GetColumns_PrimaryKeyDetectionForActivityTable_UsesActivityIdColumnName()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataRepository>>();

            var primaryKeyAttribute = new UniqueIdentifierAttributeMetadata
            {
                LogicalName = "activityid"
            };
            typeof(AttributeMetadata)
                .GetProperty("IsValidForRead")!
                .SetValue(primaryKeyAttribute, true);
            typeof(AttributeMetadata)
                .GetProperty("IsPrimaryId")!
                .SetValue(primaryKeyAttribute, true);

            var entityMetadata = new EntityMetadata
            {
                LogicalName = "activity",
                IsActivity = true
            };

            typeof(EntityMetadata)
                .GetProperty("Attributes")!
                .SetValue(entityMetadata, new AttributeMetadata[] { primaryKeyAttribute });

            var response = new RetrieveEntityResponse
            {
                Results = new ParameterCollection
                {
                    ["EntityMetadata"] = entityMetadata
                }
            };

            mockService.Execute(Arg.Any<RetrieveEntityRequest>()).Returns(response);

            var metadataRepo = new MetadataRepository(mockService, mockLogger);

            // Act
            var result = metadataRepo.GetColumns("activity");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("activityid", result[0].Name);
            Assert.True(result[0].IsPrimaryKey);
        }

        [Fact]
        public void GetColumns_PrimaryKeyDetectionForActivityTable_IgnoresTableNamePattern()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataRepository>>();

            var primaryKeyAttribute = new UniqueIdentifierAttributeMetadata
            {
                LogicalName = "activityid"
            };
            typeof(AttributeMetadata)
                .GetProperty("IsValidForRead")!
                .SetValue(primaryKeyAttribute, true);
            typeof(AttributeMetadata)
                .GetProperty("IsPrimaryId")!
                .SetValue(primaryKeyAttribute, true);

            var otherAttribute = new UniqueIdentifierAttributeMetadata
            {
                LogicalName = "phonecallid"
            };
            typeof(AttributeMetadata)
                .GetProperty("IsValidForRead")!
                .SetValue(otherAttribute, true);
            typeof(AttributeMetadata)
                .GetProperty("IsPrimaryId")!
                .SetValue(otherAttribute, false);

            var entityMetadata = new EntityMetadata
            {
                LogicalName = "phonecall",
                IsActivity = true
            };

            typeof(EntityMetadata)
                .GetProperty("Attributes")!
                .SetValue(entityMetadata, new AttributeMetadata[] { primaryKeyAttribute, otherAttribute });

            var response = new RetrieveEntityResponse
            {
                Results = new ParameterCollection
                {
                    ["EntityMetadata"] = entityMetadata
                }
            };

            mockService.Execute(Arg.Any<RetrieveEntityRequest>()).Returns(response);

            var metadataRepo = new MetadataRepository(mockService, mockLogger);

            // Act
            var result = metadataRepo.GetColumns("phonecall");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.True(result[0].IsPrimaryKey);
            Assert.Equal("activityid", result[0].Name);
            Assert.False(result[1].IsPrimaryKey);
            Assert.Equal("phonecallid", result[1].Name);
        }

        [Fact]
        public void GetColumns_FailedExecute_ThrowsFaultException()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataRepository>>();
            var faultException = new FaultException<OrganizationServiceFault>(new OrganizationServiceFault
            {
                ErrorCode = -2147220969,
                Message = "An error occurred while processing the request."
            });
            mockService.Execute(Arg.Any<RetrieveEntityRequest>()).Throws(faultException);
            var metadataRepo = new MetadataRepository(mockService, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ReadMetadataException>(() => metadataRepo.GetColumns("account"));
            Assert.IsType<FaultException<OrganizationServiceFault>>(exception.InnerException);
        }

        [Fact]
        public void GetColumns_FailedExecute_ThrowsTimeoutException()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataRepository>>();
            mockService.Execute(Arg.Any<RetrieveEntityRequest>()).Throws(new TimeoutException("The operation has timed out."));
            var metadataRepo = new MetadataRepository(mockService, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ReadMetadataException>(() => metadataRepo.GetColumns("account"));
            Assert.IsType<TimeoutException>(exception.InnerException);
        }

        [Fact]
        public void GetColumns_FailedExecute_ThrowsGenericException()
        {
            // Arrange
            var mockService = Substitute.For<IOrganizationService>();
            var mockLogger = Substitute.For<ILogger<MetadataRepository>>();
            mockService.Execute(Arg.Any<RetrieveEntityRequest>()).Throws(new Exception("Unexpected error"));
            var metadataRepo = new MetadataRepository(mockService, mockLogger);

            // Act & Assert
            var exception = Assert.Throws<ReadMetadataException>(() => metadataRepo.GetColumns("account"));
            Assert.IsType<Exception>(exception.InnerException);
        }
    }
}

