using System;
using System.Collections.Generic;
using System.Text;

namespace Pg.DataverseSync.Engine.Core.Model
{
    public interface IChecksumSource
    {
        string GetChecksum();
    }
}
