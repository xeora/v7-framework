namespace Xeora.Web.Basics.Domain.Control.Definitions
{
    public interface ICheckbox : IBase, IUpdates, IHasAttributes
    {
        string Text { get; }
    }
}