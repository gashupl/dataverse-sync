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

        public static string SerializeToJsonString<T>(T obj)
        {
            using var ms = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(ms, obj);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
