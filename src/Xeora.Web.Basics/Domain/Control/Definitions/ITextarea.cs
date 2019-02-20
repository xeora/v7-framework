namespace Xeora.Web.Basics.Domain.Control.Definitions
{
    public interface ITextarea : IBase, IUpdates, IHasAttributes
    {
        string Content { get; }
    }
}