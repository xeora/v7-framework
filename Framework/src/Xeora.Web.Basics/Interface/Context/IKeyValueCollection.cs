namespace Xeora.Web.Basics.Context
{
    public interface IKeyValueCollection<K, V>
    {
        K[] Keys { get; }
        V this[K key] { get; }
    }
}
