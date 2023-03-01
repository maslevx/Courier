using Newtonsoft.Json;

namespace Courier.Bus
{
    public class JsonSerializer : ISerializer
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public string Serialize<T>(T message)
        {
            return JsonConvert.SerializeObject(message, JsonSettings);
        }
    }
}
