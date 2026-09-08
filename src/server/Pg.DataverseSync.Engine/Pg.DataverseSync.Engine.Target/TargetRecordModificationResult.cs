using System;
using System.Collections.Generic;
using System.Text;

namespace Pg.DataverseSync.Engine.Target
{
    public class TargetRecordModificationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
