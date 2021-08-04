namespace Harry.Caching
{
    public interface IConverter
    {
        byte[] Serialize<TModel>(TModel value);

        TModel Deserialize<TModel>(byte[] value);
    }
}
