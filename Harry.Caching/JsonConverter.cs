using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Harry.Caching
{
    /// <summary>
    /// Json转换器
    /// </summary>
    public class JsonConverter : IConverter
    {
        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            //不进行转义.如汉字
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            //忽略循环引用
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            // 忽略值为Null的属性
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            // 反序列化不区分大小写
            PropertyNameCaseInsensitive = true,
            // 忽略只读属性，因为只读属性只能序列化而不能反序列化，所以在以json为储存数据的介质的时候，序列化只读属性意义不大
            IgnoreReadOnlyFields = true,
        };

        public byte[] Serialize<TModel>(TModel value)
        {
            if (value == null)
                return null;

            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, options));
        }

        public TModel Deserialize<TModel>(byte[] value)
        {
            if (value == null)
                return default;

            return JsonSerializer.Deserialize<TModel>(Encoding.UTF8.GetString(value));
        }
    }
}
