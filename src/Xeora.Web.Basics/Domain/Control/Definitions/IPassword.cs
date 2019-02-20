namespace Xeora.Web.Basics.Domain.Control.Definitions
{
    public interface IPassword : IBase, IUpdates, IHasAttributes
    {
        string Text { get; }
        string DefaultButtonID { get; }
    }
}