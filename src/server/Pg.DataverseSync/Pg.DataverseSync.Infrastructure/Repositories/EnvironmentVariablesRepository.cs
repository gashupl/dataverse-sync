using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Domain.Repositories;
using Pg.DataverseSync.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pg.DataverseSync.Infrastructure.Repositories
{
    public class EnvironmentVariablesRepository : DataRepository, IEnvironmentVariablesRepository
    {
        public EnvironmentVariablesRepository(IOrganizationServiceFactory orgSvcFactory, ITracingService tracingService) 
            : base(orgSvcFactory, tracingService)
        {
        }

        public string GetValue(string name)
        {
            using (var context = new DataverseContext(service))
            {
                var value = (from ev in context.EnvironmentVariableValueSet
                             join ed in context.EnvironmentVariableDefinitionSet
                             on ev.EnvironmentVariableDefinitionId.Id equals ed.Id
                             where ed.SchemaName == name
                             select ev.Value).FirstOrDefault();

                return value;
            }
        }
    }
}
