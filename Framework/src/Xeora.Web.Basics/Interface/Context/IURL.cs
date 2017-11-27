namespace Xeora.Web.Basics.Context
{
    public interface IURL
    {
        string Raw { get; }
        string Relative { get; }
        string RelativePath { get; }
        string QueryString { get; }
    }
}
