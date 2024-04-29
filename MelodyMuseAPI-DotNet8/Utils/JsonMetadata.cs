using System.Text.Json;

namespace MelodyMuseAPI.Utils
{
    public class JsonMetadata
    {
        private JsonElement _element;

        public JsonMetadata(string json)
        {
            using (var doc = JsonDocument.Parse(json))
            {
                _element = doc.RootElement.Clone();
            }
        }

        public string GetValue(string key)
        {
            if (_element.TryGetProperty(key, out JsonElement value))
            {
                return value.ToString();
            }
            return null;
        }
    }

}
