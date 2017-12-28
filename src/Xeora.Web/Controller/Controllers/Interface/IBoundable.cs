namespace Xeora.Web.Controller.Directive
{
    public interface IBoundable
    {
        bool HasBound { get; }
        string BoundControlID { get; }
    }
}