namespace Xeora.Web.Basics.Context
{
    public interface IUrl
    {
        string Raw { get; }
        string Relative { get; }
        string RelativePath { get; }
        string QueryString { get; }
    }
}
