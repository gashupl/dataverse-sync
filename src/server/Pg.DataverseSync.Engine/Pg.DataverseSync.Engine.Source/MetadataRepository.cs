using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Pg.DataverseSync.Engine.Core.Exceptions;
using Pg.DataverseSync.Engine.Core.Model;
using Pg.DataverseSync.Engine.Application.Data;
using System.ServiceModel;

namespace Pg.DataverseSync.Engine.Source
{
    public class MetadataRepository : DataRepositoryBase, IMetadataRepository
    {
        public MetadataRepository(IOrganizationService service, ILogger<MetadataRepository> logger) 
            : base(service, logger)
        {
        }

        public List<Table>? GetTables()
        {
            LogIfEnabled(LogLevel.Information, "Retrieving table metadata from Dataverse...");

            try
            {
                var request = new RetrieveAllEntitiesRequest
                {
                    EntityFilters = EntityFilters.Entity,
                    RetrieveAsIfPublished = true
                };

                var response = (RetrieveAllEntitiesResponse)service.Execute(request);
                var tables = new List<Table>();

                foreach (var entityMetadata in response.EntityMetadata)
                {
                    string logicalName = entityMetadata.LogicalName;
                    string displayName = entityMetadata.DisplayName?.UserLocalizedLabel?.Label ?? logicalName;
                    bool isActivity = entityMetadata.IsActivity.GetValueOrDefault(false);

                    tables.Add(new Table(logicalName, displayName, isActivity));
                }

                LogIfEnabled(LogLevel.Information, "Retrieved {TableCount} tables from Dataverse.", tables.Count);
                return tables;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                var msg = $"Dataverse service fault while retrieving tables. Error code: {ex.Detail.ErrorCode}, Message: {ex.Detail.Message}"; 
                LogIfEnabled(LogLevel.Error, ex, msg);
                throw new ReadMetadataException(msg, ex); 
            }
            catch (TimeoutException ex)
            {
                var msg = "Timeout while retrieving tables from Dataverse. Consider increasing the timeout settings.";
                LogIfEnabled(LogLevel.Error, ex, msg);
                throw new ReadMetadataException(msg, ex);
            }
            catch (Exception ex)
            {
                var msg = "An unexpected error occurred while retrieving tables from Dataverse.";
                LogIfEnabled(LogLevel.Error, ex, msg);
                throw new ReadMetadataException(msg, ex);
            }
        }

        public List<Column> GetColumns(string tableName)
        {
            LogIfEnabled(LogLevel.Information, "Retrieving columns metadata from Dataverse table {TableName}...", tableName);

            try
            {
                var request = new RetrieveEntityRequest
                {
                    LogicalName = tableName,
                    EntityFilters = EntityFilters.Attributes,
                    RetrieveAsIfPublished = true
                };

                var response = (RetrieveEntityResponse)service.Execute(request);
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
                var msg = $"Dataverse service fault while retrieving columns metadata from Dataverse table {tableName}. Error code: {ex.Detail.ErrorCode}, Message: {ex.Detail.Message}";
                LogIfEnabled(LogLevel.Error, ex, msg);
                throw new ReadMetadataException(msg, ex);
            }
            catch (TimeoutException ex)
            {
                var msg = 
                    $"Timeout while retrieving columns metadata from Dataverse table {tableName}. Consider increasing the timeout settings.";
                LogIfEnabled(LogLevel.Error, ex, msg);
                throw new ReadMetadataException(msg, ex);
            }
            catch (Exception ex)
            {
                var msg = 
                    $"An unexpected error occurred while retrieving columns metadata from Dataverse table {tableName}";
                LogIfEnabled(LogLevel.Error, ex, msg);
                throw new ReadMetadataException(msg, ex);
            }

           
        }
    }
}
