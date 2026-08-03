using System;
using System.Collections.Generic;
using System.Text;

namespace Pg.DataverseSync.Engine.Application.Data
{
    public interface ITargetSchemaRepository
    {
        bool TargetTableExists(string tableName); 
    }
}
