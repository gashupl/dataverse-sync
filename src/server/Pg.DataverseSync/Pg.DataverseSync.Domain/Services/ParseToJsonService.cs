using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;

namespace Pg.DataverseSync.Domain.Services
{
    public class ParseToJsonService : ServiceBase, IParseToJsonService
    {
        public ParseToJsonService(ITracingService tracingService) : base(tracingService)
        {
        }

        public string Parse(object data)
        {
            if (data == null)
            {
                tracingService.Trace($"Data argument is null");
                throw new ArgumentNullException(nameof(data));
            }

            tracingService.Trace($"ParseToJsonService.Parse executed with data object: {data.ToString()}");

            var json = JsonConvert.SerializeObject(data);
            tracingService.Trace($"ParseToJsonService.Parse execution completed.");
            return json; 
        }
    }
}

