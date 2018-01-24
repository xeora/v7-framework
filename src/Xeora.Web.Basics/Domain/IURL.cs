using Xeora.Web.Basics.Mapping;

namespace Xeora.Web.Basics.Domain
{
    public interface IURL
    {
        bool Active { get; }
        string ResolverExecutable { get; }
        MappingItemCollection Items { get; }
    }
}
