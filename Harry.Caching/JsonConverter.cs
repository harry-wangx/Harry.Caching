using System.Text;
using System.Text.Json;

namespace Harry.Caching
{
    /// <summary>
    /// Json转换器
    /// </summary>
    public class JsonConverter : IConverter
    {
        public byte[] Serialize<TModel>(TModel value)
        {
            if (value == null)
                return null;

            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
        }

        public TModel Deserialize<TModel>(byte[] value)
        {
            if (value == null)
                return default;

            return JsonSerializer.Deserialize<TModel>(Encoding.UTF8.GetString(value));
        }
    }
}
