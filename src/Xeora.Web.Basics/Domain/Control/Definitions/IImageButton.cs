namespace Xeora.Web.Basics.Domain.Control.Definitions
{
    public interface IImageButton : IBase, IUpdates, IHasAttributes
    {
        string Source { get; }
    }
}