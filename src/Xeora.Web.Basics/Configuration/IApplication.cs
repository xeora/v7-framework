namespace Xeora.Web.Basics.Configuration
{
    public interface IApplication
    {
        IMain Main { get; }
        IRequestTagFilter RequestTagFilter { get; }
        IMimeItem[] CustomMimes { get; }
        string[] BannedFiles { get; }
    }
}
