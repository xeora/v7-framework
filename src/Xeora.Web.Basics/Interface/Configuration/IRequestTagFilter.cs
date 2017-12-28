namespace Xeora.Web.Basics.Configuration
{
    public interface IRequestTagFilter
    {
        RequestTagFilteringTypes Direction { get; }
        string[] Items { get; }
        string[] Exceptions { get; }
    }
}
