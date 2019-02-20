namespace Xeora.Web.Basics.Domain.Control.Definitions
{
    public interface IRadioButton : IBase, IUpdates, IHasAttributes
    {
        string Text { get; }
    }
}