namespace Xeora.Web.Basics.Domain.Control.Definitions
{
    public interface IButton : IBase, IUpdates, IHasAttributes
    {
        string Text { get; }
    }
}