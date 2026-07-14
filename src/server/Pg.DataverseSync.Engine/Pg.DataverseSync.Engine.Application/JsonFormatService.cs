using System.Text;
using System.Runtime.Serialization.Json; 

namespace Pg.DataverseSync.Engine.Application
{
    public class JsonFormatService
    {
        public static T DeserializeJsonString<T>(string jsonString)
        {
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(ms)!;
        }
    }
}
