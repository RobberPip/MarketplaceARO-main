using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace RegistryApp.Utils.Helpers {
    public class JsonWorker {
        private static JsonSerializerOptions serializerOptions = new JsonSerializerOptions() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public static string SerializeObject<T>(T model) => JsonSerializer.Serialize(model, serializerOptions);

        public static T DeSerializeObject<T>(string data) => JsonSerializer.Deserialize<T>(data);
    }
}
