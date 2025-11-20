using Microsoft.Xrm.Sdk;
using System;

namespace Pg.DataverseSync.Infrastructure.Tests.Core
{
    public class FakeOrganizationServiceFactory : IOrganizationServiceFactory
    {
        private readonly IOrganizationService service; 
        public FakeOrganizationServiceFactory(IOrganizationService service)
        {
            this.service = service; 
        }
        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            return this.service; 
        }
    }
}

