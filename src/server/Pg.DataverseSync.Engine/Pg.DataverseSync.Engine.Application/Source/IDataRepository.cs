using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pg.DataverseSync.Engine.Application.Source
{
    public interface IDataRepository
    {
        //TODO: Add methods for sync tables retrieval 
        List<Entity> GetRecords(string tableName, List<string> columns); 
    }
}
