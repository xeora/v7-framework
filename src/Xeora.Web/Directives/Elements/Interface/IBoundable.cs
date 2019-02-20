namespace Xeora.Web.Directives.Elements
{
    public interface IBoundable
    {
        bool HasBound { get; }
        string BoundDirectiveID { get; }
    }
}