using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.Caching
{
    public interface IConverter
    {
        byte[] Serialize<T>(T value);

        T Deserialize<T>(byte[] value);
    }
}
