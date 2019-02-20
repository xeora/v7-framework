namespace Xeora.Web.Directives.Controls
{
    public interface IControl
    {
        bool Searchable { get; }

        void Parse();
        void Render(string requesterUniqueID);
    }
}