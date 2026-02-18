using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Domain.Source;
using System.ServiceModel;

namespace Pg.DataverseSync.Engine.Source
{
    public class MetadataReader : IMetadataReader
    {
        private readonly IOrganizationService _service;
        private readonly ILogger<MetadataReader> _logger;

        public MetadataReader(IOrganizationService service, ILogger<MetadataReader> logger)
        {
            _service = service; 
            _logger = logger;
        }

        public List<Table>? GetTables()
        {
            _logger.LogInformation("Retrieving table metadata from Dataverse...");

            try
            {
                var request = new RetrieveAllEntitiesRequest
                {
                    EntityFilters = EntityFilters.Entity,
                    RetrieveAsIfPublished = true
                };

                var response = (RetrieveAllEntitiesResponse)_service.Execute(request);
                var tables = new List<Table>();

                foreach (var entityMetadata in response.EntityMetadata)
                {
                    string logicalName = entityMetadata.LogicalName;
                    string displayName = entityMetadata.DisplayName?.UserLocalizedLabel?.Label ?? logicalName;
                    bool isActivity = entityMetadata.IsActivity.GetValueOrDefault(false);

                    tables.Add(new Table(logicalName, displayName, isActivity));
                }

                _logger.LogInformation("Retrieved {Count} tables from Dataverse.", tables.Count);
                return tables;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                var msg = $"Dataverse service fault while retrieving tables. " +
                    $"Error code: {ex.Detail.ErrorCode}, Message: {ex.Detail.Message}"; 
                _logger.LogError(ex, msg);
                throw new ReadMetadataException(msg, ex); 
            }
            catch (TimeoutException ex)
            {
                var msg = "Timeout while retrieving tables from Dataverse. Consider increasing the timeout settings.";
                _logger.LogError(ex, msg);
                throw new ReadMetadataException(msg, ex);
            }
            catch (Exception ex)
            {
                var msg = "An unexpected error occurred while retrieving tables from Dataverse.";
                _logger.LogError(ex, msg);
                throw new ReadMetadataException(msg, ex);
            }
        }

        public List<Column> GetColumns(string tableName)
        {
            _logger.LogInformation("Retrieving columns metadata from Dataverse table {tableName}...", tableName);

            try
            {
                var request = new RetrieveEntityRequest
                {
                    LogicalName = tableName,
                    EntityFilters = EntityFilters.Attributes,
                    RetrieveAsIfPublished = true
                };

                var response = (RetrieveEntityResponse)_service.Execute(request);
                var columns = new List<Column>();

                foreach (var attributeMetadata in response.EntityMetadata.Attributes)
                {
                    string name = attributeMetadata.LogicalName;
                    bool isPrimaryKey = attributeMetadata.IsPrimaryId.GetValueOrDefault(false);
                    string? dataType = attributeMetadata.AttributeTypeName?.Value;

                    columns.Add(new Column(name, dataType, isPrimaryKey, isNullable: true));
                }

                return columns;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                var msg = $"Dataverse service fault while retrieving columns metadata from  Dataverse table {tableName}. " +
                    $"Error code: {ex.Detail.ErrorCode}, Message: {ex.Detail.Message}";
                _logger.LogError(ex, msg);
                throw new ReadMetadataException(msg, ex);
            }
            catch (TimeoutException ex)
            {
                var msg = 
                    $"Timeout while retrieving columns metadata from Dataverse table {tableName}. Consider increasing the timeout settings.";
                _logger.LogError(ex, msg);
                throw new ReadMetadataException(msg, ex);
            }
            catch (Exception ex)
            {
                var msg = 
                    $"An unexpected error occurred while retrieving columns metadata from Dataverse table {tableName}";
                _logger.LogError(ex, msg);
                throw new ReadMetadataException(msg, ex);
            }

           
        }
    }
}
