namespace Xeora.Web.Directives.Controls
{
    public interface IControl
    {
        DirectiveCollection Children { get; }
        bool LinkArguments { get; }

        void Parse();
        void Render(string requesterUniqueID);
    }
}