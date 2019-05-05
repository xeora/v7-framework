namespace Xeora.Web.Directives.Controls
{
    public interface IControl
    {
        DirectiveCollection Children { get; }

        void Parse();
        void Render(string requesterUniqueID);
    }
}