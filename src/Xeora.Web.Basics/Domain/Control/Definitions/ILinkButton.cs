namespace Xeora.Web.Basics.Domain.Control.Definitions
{
    public interface ILinkButton : IBase, IUpdates, IHasAttributes
    {
        string Text { get; }
        string Url { get; }
    }
}