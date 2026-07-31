using System;
using System.Collections.Generic;
using System.Text;

namespace Pg.DataverseSync.Engine.Core.Schema
{
    //See ADR-0003: docs/adr/0003-lightweight-manual-created-schema.md
    public static class SyncTable
    {
        public const string EntityName = "pg_synctable";

        public static class Columns
        {
            public const string Name = "pg_name";
            public const string StateCode = "statecode";    
        }

        public static class StateCode
        {
            public const int Active = 0;
            public const int Inactive = 1;
        }
    }
}
