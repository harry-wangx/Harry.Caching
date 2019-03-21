using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.Caching
{
    public class JsonConverter : IConverter
    {
        public byte[] Serialize<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(value));
        }

        public T Deserialize<T>(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(value));
        }
    }
}
