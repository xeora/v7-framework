namespace Xeora.Web.Basics
{
    public interface IURLMappings
    {
        bool IsActive { get; }
        string ResolverExecutable { get; }
        URLMapping.URLMappingItem.URLMappingItemCollection Items { get; }
    }
}
