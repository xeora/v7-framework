namespace Xeora.Web.Basics.Domain.Control.Definitions
{
    public interface ITextbox : IBase, IUpdates, IHasAttributes
    {
        string Text { get; }
        string DefaultButtonId { get; }
    }
}